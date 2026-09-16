using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }
    
    public List<BuildingDataSO> availableBuildings;
    
    public bool isBuildModeActive = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        availableBuildings = Resources.LoadAll<BuildingDataSO>("Buildings")
            .Where(b => b.buildingType != BuildingType.Mine && b.buildingType != BuildingType.Entrance && b.buildingType != BuildingType.Hall)
            .ToList();
    }
    
    public int GetRemainingCount(string buildingName)
    {
        int hallLevel = 1;
        if (GridManager.Instance != null)
        {
            var hall = GridManager.Instance.GetAllBuildings().FirstOrDefault(b => b.data.buildingType == BuildingType.Hall);
            if (hall != null) hallLevel = hall.level;
        }
        
        int allowed = 0;
        
        // Lv 1 base limits
        if (buildingName == "StoneFactory") allowed += 2;
        if (buildingName == "Road") allowed += 10;
        if (buildingName == "Slingshot") allowed += 2;
        if (buildingName == "HammerFactory") allowed += 5; // Give them 5 to build in village
        
        // Lv 2 limits (궁수 X3, 화살공장 X 3, 도로 X 4)
        if (hallLevel >= 2)
        {
            if (buildingName == "Archer") allowed += 3;
            if (buildingName == "ArrowFactory") allowed += 3;
            if (buildingName == "Road") allowed += 4;
            if (buildingName == "SlowEffect") allowed += 3; 
            if (buildingName == "DamageEffect") allowed += 3;
        }
        
        // Lv 3 limits (총알 공장 X 3, 도로 X 15, 총 X1)
        if (hallLevel >= 3)
        {
            if (buildingName == "AmmoFactory") allowed += 3;
            if (buildingName == "Road") allowed += 15;
            if (buildingName == "Gun") allowed += 1;
        }
        
        // Lv 4 limits (공장 버프 X3, 타워버프 X2, 총 X3)
        if (hallLevel >= 4)
        {
            if (buildingName == "FactoryBuff") allowed += 3;
            if (buildingName == "TowerBuff") allowed += 2;
            if (buildingName == "Gun") allowed += 3;
        }
        
        int built = 0;
        if (GridManager.Instance != null)
            built = GridManager.Instance.GetAllBuildings().Count(b => b.data.buildingName == buildingName);
            
        return allowed - built;
    }
    
    public bool IsPlacing => _isPlacing;
    public BuildingDataSO BuildingToPlace => _buildingToPlace;
    
    private bool _isPlacing = false;
    private BuildingDataSO _buildingToPlace = null;
    
    public void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;
        if (!isBuildModeActive) CancelPlacement();
        else
        {
            var gridRenderer = Object.FindObjectOfType<GridRenderer>();
            if (gridRenderer != null) gridRenderer.SelectedBuilding = null;
        }
    }
    
    void Update()
    {
        if (!isBuildModeActive) return;
        
        if (_isPlacing && _buildingToPlace != null && Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
                return;
                
            var cam = Camera.main ?? Object.FindObjectOfType<Camera>();
            if (cam == null) return;
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            int posX = Mathf.RoundToInt(mousePos.x);
            int posY = Mathf.RoundToInt(mousePos.y);
            
            if (IsValidPlacement(posX, posY))
            {
                int cost = _buildingToPlace.cost;
                
                if (GameState.HasResource(_buildingToPlace.costType, cost) && GetRemainingCount(_buildingToPlace.buildingName) > 0)
                {
                    GameState.ConsumeResource(_buildingToPlace.costType, cost);
                    bool placed = GridManager.Instance.PlaceBuilding(_buildingToPlace, posX, posY);
                    if (!placed)
                    {
                        GameState.RefundResource(_buildingToPlace.costType, cost);
                        CancelPlacement();
                    }
                }
                else
                {
                    CancelPlacement();
                }
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }
    
    public void StartPlacement(BuildingDataSO data)
    {
        if (GetRemainingCount(data.buildingName) <= 0) return;
        
        _buildingToPlace = data;
        _isPlacing = true;
    }
    
    public void CancelPlacement()
    {
        _buildingToPlace = null;
        _isPlacing = false;
    }
    
    public bool IsValidPlacement(int posX, int posY)
    {
        if (GridManager.Instance.GetBuildingAt(posX, posY) != null) return false;
        if (_buildingToPlace == null) return false;
        
        return GridManager.Instance.IsValidCoordinate(posX, posY, _buildingToPlace);
    }
}
