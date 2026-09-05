using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    
    public List<BuildingDataSO> fullDeck;
    private Queue<BuildingDataSO> _deckQueue;
    public BuildingDataSO[] currentHand = new BuildingDataSO[2];
    
    public int summonCost = 100;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (fullDeck == null || fullDeck.Count == 0)
        {
            var loadedBuildings = Resources.LoadAll<BuildingDataSO>("Buildings");
            fullDeck = new List<BuildingDataSO>();
            foreach (var building in loadedBuildings)
            {
                if (building.buildingType != BuildingType.Road)
                {
                    fullDeck.Add(building);
                }
            }
        }
        
        _deckQueue = new Queue<BuildingDataSO>(fullDeck);
        
        // Draw initial hand
        for (int i = 0; i < 2; i++)
        {
            if (_deckQueue.Count > 0)
                currentHand[i] = _deckQueue.Dequeue();
        }
    }
    
    private bool _isPlacing = false;
    private BuildingDataSO _buildingToPlace = null;
    private int _handIndexBeingPlaced = -1;
    
    void Update()
    {
        if (_isPlacing && _buildingToPlace != null && Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
                return;
                
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int posX = Mathf.RoundToInt(mousePos.x);
            int posY = Mathf.RoundToInt(mousePos.y);
            
            bool isValidPos = true;
            if (_buildingToPlace.buildingType == BuildingType.Factory || _buildingToPlace.buildingType == BuildingType.FactorySpeedBuff)
            {
                if (posX < 3 || posX > 6 || posY < 3 || posY > 6) isValidPos = false;
            }
            else if (_buildingToPlace.buildingType == BuildingType.Tower || _buildingToPlace.buildingType == BuildingType.TowerAttackBuff)
            {
                if (posX >= 3 && posX <= 6 && posY >= 3 && posY <= 6) isValidPos = false;
            }
            
            if (isValidPos)
            {
                int cost = _handIndexBeingPlaced == -1 ? 50 : summonCost;
                
                if (DefenseManager.Instance.SpendGold(cost))
                {
                    bool placed = GridManager.Instance.PlaceBuilding(_buildingToPlace, posX, posY);
                    if (placed)
                    {
                        if (_handIndexBeingPlaced != -1)
                        {
                            _deckQueue.Enqueue(_buildingToPlace);
                            currentHand[_handIndexBeingPlaced] = _deckQueue.Count > 0 ? _deckQueue.Dequeue() : null;
                        }
                    }
                    else
                    {
                        DefenseManager.Instance.AddGold(cost); 
                    }
                }
            }
            
            _isPlacing = false;
            _buildingToPlace = null;
        }
    }
    
    public void SummonFromHand(int index)
    {
        if (index < 0 || index >= 2) return;
        var data = currentHand[index];
        if (data == null) return;
        
        _buildingToPlace = data;
        _handIndexBeingPlaced = index;
        _isPlacing = true;
    }
    
    public void BuildRoad()
    {
        var roadData = Resources.Load<BuildingDataSO>("Buildings/Road");
        if (roadData == null) return;
        
        _buildingToPlace = roadData;
        _handIndexBeingPlaced = -1;
        _isPlacing = true;
    }
}
