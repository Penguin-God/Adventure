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
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public IEnumerable<BuildingModel> GetAllBuildings() => _buildings.Values;
    
    public BuildingModel GetBuildingAt(int gridX, int gridY) => _buildings.Values.FirstOrDefault(building => building.x == gridX && building.y == gridY);
    
    public bool PlaceBuilding(BuildingDataSO buildingData, int gridX, int gridY)
    {
        if (GetBuildingAt(gridX, gridY) != null) return false; 
        if (gridX < 0 || gridX >= 10 || gridY < 0 || gridY >= 10) return false; 
        
        string newId = System.Guid.NewGuid().ToString();
        var buildingModel = new BuildingModel(newId, gridX, gridY, buildingData);
        _buildings[newId] = buildingModel;
        
        OnBuildingPlaced?.Invoke(buildingModel);
        OnGridChanged?.Invoke();
        return true;
    }
    
    public void RemoveBuilding(string buildingId)
    {
        if (_buildings.ContainsKey(buildingId))
        {
            _buildings.Remove(buildingId);
            OnGridChanged?.Invoke();
        }
    }
    
    public bool MoveBuilding(string buildingId, int targetX, int targetY)
    {
        if (!_buildings.ContainsKey(buildingId)) return false;
        if (GetBuildingAt(targetX, targetY) != null) return false;
        if (targetX < 0 || targetX >= 10 || targetY < 0 || targetY >= 10) return false;
        
        _buildings[buildingId].x = targetX;
        _buildings[buildingId].y = targetY;
        OnGridChanged?.Invoke();
        return true;
    }
}
