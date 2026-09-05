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
            fullDeck = new List<BuildingDataSO>(Resources.LoadAll<BuildingDataSO>("Buildings"));
        }
        
        _deckQueue = new Queue<BuildingDataSO>(fullDeck);
        
        // Draw initial hand
        for (int i = 0; i < 2; i++)
        {
            if (_deckQueue.Count > 0)
                currentHand[i] = _deckQueue.Dequeue();
        }
    }
    
    public void SummonFromHand(int index)
    {
        if (index < 0 || index >= 2) return;
        var data = currentHand[index];
        if (data == null) return;
        
        if (DefenseManager.Instance.SpendGold(summonCost))
        {
            bool placed = false;
            int maxAttempts = 100;
            
            while (!placed && maxAttempts > 0)
            {
                int posX, posY;
                if (data.buildingType == BuildingType.Factory || data.buildingType == BuildingType.FactorySpeedBuff)
                {
                    posX = Random.Range(3, 7);
                    posY = Random.Range(3, 7);
                }
                else
                {
                    posX = Random.Range(0, 10);
                    posY = Random.Range(0, 10);
                    if (posX >= 3 && posX <= 6 && posY >= 3 && posY <= 6) continue; 
                }
                
                placed = GridManager.Instance.PlaceBuilding(data, posX, posY);
                maxAttempts--;
            }
            
            _deckQueue.Enqueue(data); 
            
            // Draw next
            currentHand[index] = _deckQueue.Count > 0 ? _deckQueue.Dequeue() : null;
        }
    }
    
    private bool _isPlacingRoad = false;
    
    void Update()
    {
        if (_isPlacingRoad && Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
                return;
                
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int posX = Mathf.RoundToInt(mousePos.x);
            int posY = Mathf.RoundToInt(mousePos.y);
            
            var roadData = Resources.Load<BuildingDataSO>("Buildings/Road");
            if (roadData != null)
            {
                if (DefenseManager.Instance.SpendGold(50))
                {
                    bool placed = GridManager.Instance.PlaceBuilding(roadData, posX, posY);
                    if (!placed)
                    {
                        DefenseManager.Instance.AddGold(50); // Refund
                    }
                }
            }
            _isPlacingRoad = false;
        }
    }
    
    public void BuildRoad()
    {
        _isPlacingRoad = true;
    }
}
