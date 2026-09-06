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
    
    public bool IsPlacing => _isPlacing;
    public BuildingDataSO BuildingToPlace => _buildingToPlace;
    
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
            
            if (IsValidPlacement(posX, posY))
            {
                int cost = _buildingToPlace.cost;
                
                if (DefenseManager.Instance.SpendGold(cost))
                {
                    bool placed = GridManager.Instance.PlaceBuilding(_buildingToPlace, posX, posY);
                    if (placed)
                    {
                        if (_handIndexBeingPlaced != -1)
                        {
                            _deckQueue.Enqueue(_buildingToPlace);
                            currentHand[_handIndexBeingPlaced] = _deckQueue.Count > 0 ? _deckQueue.Dequeue() : null;
                            _isPlacing = false; // 일반 건물은 설치 시 모드 취소
                            _buildingToPlace = null;
                        }
                        // 도로는 모드 유지
                    }
                    else
                    {
                        DefenseManager.Instance.AddGold(cost); 
                    }
                }
            }
            else
            {
                // 유효하지 않은 위치 클릭 시 취소로직 없음 (피드백 기획: 연한 빨간색 표기)
            }
        }
    }
    
    public bool IsValidPlacement(int posX, int posY)
    {
        if (posX < 0 || posX >= 10 || posY < 0 || posY >= 10) return false;
        if (GridManager.Instance.GetBuildingAt(posX, posY) != null) return false;
        
        if (_buildingToPlace == null) return false;
        
        if (_buildingToPlace.buildingType == BuildingType.Road)
        {
            // 도로는 상하좌우 중에 공장이나 다른 도로가 있어야 함
            bool hasAdjacent = false;
            var adjacentPositions = new (int x, int y)[] { (posX-1, posY), (posX+1, posY), (posX, posY-1), (posX, posY+1) };
            foreach (var pos in adjacentPositions)
            {
                var building = GridManager.Instance.GetBuildingAt(pos.x, pos.y);
                if (building != null && (building.data.buildingType == BuildingType.Road || building.data.buildingType == BuildingType.Factory || building.data.buildingType == BuildingType.FactorySpeedBuff))
                {
                    hasAdjacent = true;
                    break;
                }
            }
            if (!hasAdjacent) return false;
        }
        else if (_buildingToPlace.buildingType == BuildingType.Factory)
        {
            if (posX < 3 || posX > 6 || posY < 3 || posY > 6) return false; // 4x4
        }
        else if (_buildingToPlace.buildingType == BuildingType.FactorySpeedBuff)
        {
            if (posX < 2 || posX > 7 || posY < 2 || posY > 7) return false; // 6x6
        }
        else if (_buildingToPlace.buildingType == BuildingType.Tower)
        {
            if (posX >= 2 && posX <= 7 && posY >= 2 && posY <= 7) return false; // Outer 2 lines
        }
        else if (_buildingToPlace.buildingType == BuildingType.TowerAttackBuff)
        {
            if (posX >= 3 && posX <= 6 && posY >= 3 && posY <= 6) return false; // Outer 3 lines
        }
        
        return true;
    }
    
    public void CancelPlacement()
    {
        _isPlacing = false;
        _buildingToPlace = null;
        _handIndexBeingPlaced = -1;
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
