using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }
    
    public List<BuildingDataSO> availableBuildings;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        availableBuildings = Resources.LoadAll<BuildingDataSO>("Buildings").ToList();
    }
    
    public bool IsPlacing => _isPlacing;
    public BuildingDataSO BuildingToPlace => _buildingToPlace;
    
    private bool _isPlacing = false;
    private BuildingDataSO _buildingToPlace = null;
    
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
                    if (!placed)
                    {
                        DefenseManager.Instance.AddGold(cost);
                        CancelPlacement(); // If placement fails internally
                    }
                }
                else
                {
                    CancelPlacement(); // If not enough gold
                }
            }
            else
            {
                CancelPlacement(); // If clicked invalid tile
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
    }
    
    public void StartPlacement(BuildingDataSO buildingData)
    {
        if (buildingData == null) return;
        
        _buildingToPlace = buildingData;
        _isPlacing = true;
    }
}
