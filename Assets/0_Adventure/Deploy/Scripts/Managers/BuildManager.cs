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
        availableBuildings = Resources.LoadAll<BuildingDataSO>("Buildings")
            .Where(b => b.buildingType != BuildingType.Mine && b.buildingType != BuildingType.Entrance)
            .ToList();
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
        if (GridManager.Instance.GetBuildingAt(posX, posY) != null) return false;
        if (_buildingToPlace == null) return false;
        
        return GridManager.Instance.IsValidCoordinateForType(_buildingToPlace.buildingType, posX, posY);
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
