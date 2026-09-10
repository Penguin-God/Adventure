using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    
    private Dictionary<string, BuildingModel> _buildings = new Dictionary<string, BuildingModel>();
    
    public event Action OnGridChanged;
    public event Action<BuildingModel> OnBuildingPlaced;
    public event Action<string> OnBuildingRemoved;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        // Adjust camera to center the map and shift it up to make room for UI
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(7f, 0f, -10f);
            Camera.main.orthographicSize = 9f;
        }
        
        if (GameState.savedBuildings.Count == 0)
        {
            // First time generate fixed buildings
            PlaceFixedBuilding("StoneMine", 0, 7);
            PlaceFixedBuilding("IronMine", 14, 7);
            PlaceFixedBuilding("Entrance", 0, 0);
            PlaceFixedBuilding("Entrance", 7, 0);
            PlaceFixedBuilding("Entrance", 14, 0);
        }
        else
        {
            // Load from GameState
            foreach (var b in GameState.savedBuildings)
            {
                PlaceFixedBuilding(b.buildingName, b.x, b.y, b.id, b);
            }
        }
    }
    
    private void PlaceFixedBuilding(string buildingName, int x, int y, string existingId = null, BuildingSaveData saveData = null)
    {
        var data = Resources.Load<BuildingDataSO>($"Buildings/{buildingName}");
        if (data != null)
        {
            string newId = string.IsNullOrEmpty(existingId) ? System.Guid.NewGuid().ToString() : existingId;
            var buildingModel = new BuildingModel(newId, x, y, data);
            
            if (saveData != null)
            {
                buildingModel.currentInput1 = saveData.currentInput1;
                buildingModel.currentInput2 = saveData.currentInput2;
                buildingModel.currentOutput = saveData.currentOutput;
            }
            
            _buildings[newId] = buildingModel;
            OnBuildingPlaced?.Invoke(buildingModel);
            
            if (string.IsNullOrEmpty(existingId))
                GameState.SaveGrid(_buildings.Values);
        }
    }
    
    public IEnumerable<BuildingModel> GetAllBuildings() => _buildings.Values;
    
    public BuildingModel GetBuildingAt(int gridX, int gridY) => _buildings.Values.FirstOrDefault(building => building.x == gridX && building.y == gridY);
    
    public bool IsValidCoordinateForType(BuildingType type, int gridX, int gridY)
    {
        bool isVillage = (gridX >= 0 && gridX <= 14 && gridY >= 0 && gridY <= 7);
        bool isTowerZone = false;
        
        // Tower Zone is 3 (width) x 2 (height), starting at Y=-2
        if (gridY >= -3 && gridY <= -2)
        {
            if (gridX >= 0 && gridX <= 2) isTowerZone = true;      // Entrance at 0 (X: 0, 1, 2)
            else if (gridX >= 6 && gridX <= 8) isTowerZone = true; // Entrance at 7 (X: 6, 7, 8)
            else if (gridX >= 12 && gridX <= 14) isTowerZone = true; // Entrance at 14 (X: 12, 13, 14)
        }
        
        bool isTowerType = type == BuildingType.Tower || type == BuildingType.TowerAttackBuff;
        if (isTowerType) return isTowerZone;
        
        // Allow Roads in the gap (Y=-1) at the entrance X coordinates
        if (type == BuildingType.Road && gridY == -1 && (gridX == 0 || gridX == 7 || gridX == 14)) return true;
        
        return isVillage;
    }
    
    public bool PlaceBuilding(BuildingDataSO buildingData, int gridX, int gridY)
    {
        if (GetBuildingAt(gridX, gridY) != null) return false; 
        if (!IsValidCoordinateForType(buildingData.buildingType, gridX, gridY)) return false; 
        
        string newId = System.Guid.NewGuid().ToString();
        var buildingModel = new BuildingModel(newId, gridX, gridY, buildingData);
        _buildings[newId] = buildingModel;
        
        OnBuildingPlaced?.Invoke(buildingModel);
        OnGridChanged?.Invoke();
        GameState.SaveGrid(_buildings.Values);
        return true;
    }
    
    public void RemoveBuilding(string buildingId)
    {
        if (_buildings.ContainsKey(buildingId))
        {
            _buildings.Remove(buildingId);
            OnBuildingRemoved?.Invoke(buildingId);
            OnGridChanged?.Invoke();
            GameState.SaveGrid(_buildings.Values);
        }
    }
    
    public bool MoveBuilding(string buildingId, int targetX, int targetY)
    {
        if (!_buildings.ContainsKey(buildingId)) return false;
        
        var building = _buildings[buildingId];
        // Fixed buildings cannot be moved
        if (building.data.buildingType == BuildingType.Mine || building.data.buildingType == BuildingType.Entrance) return false;
        
        if (GetBuildingAt(targetX, targetY) != null) return false;
        if (!IsValidCoordinateForType(building.data.buildingType, targetX, targetY)) return false;
        
        building.x = targetX;
        building.y = targetY;
        OnGridChanged?.Invoke();
        GameState.SaveGrid(_buildings.Values);
        return true;
    }
}
