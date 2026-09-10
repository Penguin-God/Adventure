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
            .Where(b => b.buildingType != BuildingType.Mine && b.buildingType != BuildingType.Entrance)
            .ToList();
    }
    
    public int GetRemainingCount(string buildingName)
    {
        int allowed = 0;
        for (int i = 1; i <= GameState.unlockedStage; i++)
        {
            var stageData = Resources.Load<StageDataSO>($"Stages/Stage_{i}");
            if (stageData != null)
            {
                var reward = stageData.buildingRewards.FirstOrDefault(r => r.buildingName == buildingName);
                if (reward != null) allowed += reward.count;
            }
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
                
                if (GameState.currentGold >= cost && GetRemainingCount(_buildingToPlace.buildingName) > 0)
                {
                    GameState.currentGold -= cost;
                    bool placed = GridManager.Instance.PlaceBuilding(_buildingToPlace, posX, posY);
                    if (!placed)
                    {
                        GameState.currentGold += cost;
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
        
        return GridManager.Instance.IsValidCoordinateForType(_buildingToPlace.buildingType, posX, posY);
    }
}
