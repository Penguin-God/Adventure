using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GatchTycoon.Domain;
using GatchTycoon.Data;
using System;

namespace GatchTycoon.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }
        
        public GameConfigSO gameConfig;
        public int currentCityHallLevel = 1;
        
        private Dictionary<Vector2Int, BuildingModel> _grid = new Dictionary<Vector2Int, BuildingModel>();
        public Action<BuildingModel> OnBuildingPlaced;
        public Action<BuildingModel> OnBuildingRemoved;
        public Action OnGridChanged;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public Vector2Int GetGridSize()
        {
            var levelInfo = gameConfig.cityHallLevels.FirstOrDefault(l => l.level == currentCityHallLevel);
            if (levelInfo != null)
                return new Vector2Int(levelInfo.gridSizeX, levelInfo.gridSizeY);
            return new Vector2Int(3, 3); // Updated to 3x3 default
        }
        
        public bool IsValidPosition(int x, int y)
        {
            var size = GetGridSize();
            return x >= 0 && x < size.x && y >= 0 && y < size.y;
        }
        
        public bool IsEmpty(int x, int y) => !_grid.ContainsKey(new Vector2Int(x, y));
        
        public BuildingModel GetBuildingAt(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            return _grid.ContainsKey(pos) ? _grid[pos] : null;
        }
        
        public IEnumerable<BuildingModel> GetAllBuildings() => _grid.Values;
        
        public void PlaceBuilding(BuildingDataSO data, int x, int y)
        {
            if (!IsValidPosition(x, y)) return;
            
            var pos = new Vector2Int(x, y);
            if (_grid.ContainsKey(pos)) return;
            
            string id = Guid.NewGuid().ToString();
            var building = new BuildingModel(id, data, x, y);
            _grid[pos] = building;
            
            OnBuildingPlaced?.Invoke(building);
            OnGridChanged?.Invoke();
        }
        
        public void RemoveBuilding(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            if (_grid.ContainsKey(pos))
            {
                var building = _grid[pos];
                _grid.Remove(pos);
                OnBuildingRemoved?.Invoke(building);
                OnGridChanged?.Invoke();
            }
        }
        
        public bool SwapOrMoveBuilding(int startX, int startY, int targetX, int targetY)
        {
            if (!IsValidPosition(targetX, targetY)) return false;
            
            var startPos = new Vector2Int(startX, startY);
            var targetPos = new Vector2Int(targetX, targetY);
            
            if (!_grid.ContainsKey(startPos)) return false;
            
            var sourceBuilding = _grid[startPos];
            
            if (_grid.ContainsKey(targetPos))
            {
                var targetBuilding = _grid[targetPos];
                
                // Pure swap, merge is removed from here
                _grid[startPos] = targetBuilding;
                _grid[targetPos] = sourceBuilding;
                
                sourceBuilding.x = targetX;
                sourceBuilding.y = targetY;
                targetBuilding.x = startX;
                targetBuilding.y = startY;
                
                OnGridChanged?.Invoke();
                return true;
            }
            else
            {
                // Move
                _grid.Remove(startPos);
                _grid[targetPos] = sourceBuilding;
                sourceBuilding.x = targetX;
                sourceBuilding.y = targetY;
                
                OnGridChanged?.Invoke();
                return true;
            }
        }
        
        public bool CanCombine(BuildingDataSO buildingType)
        {
            if (buildingType.nextLevelBuilding == null) return false;
            int count = _grid.Values.Count(b => b.data == buildingType);
            return count >= buildingType.requiredCount;
        }
        
        public bool ExecuteCombine(BuildingDataSO buildingType)
        {
            if (!CanCombine(buildingType)) return false;
            
            var targets = _grid.Values.Where(b => b.data == buildingType).Take(buildingType.requiredCount).ToList();
            
            var firstTarget = targets[0];
            int spawnX = firstTarget.x;
            int spawnY = firstTarget.y;
            
            foreach (var t in targets)
            {
                RemoveBuilding(t.x, t.y);
            }
            
            PlaceBuilding(buildingType.nextLevelBuilding, spawnX, spawnY);
            return true;
        }
        
        public bool UpgradeCityHall()
        {
            var nextLevel = gameConfig.cityHallLevels.FirstOrDefault(l => l.level == currentCityHallLevel + 1);
            if (nextLevel != null)
            {
                if (CurrencyManager.Instance.SpendCurrency(nextLevel.costCurrency, nextLevel.upgradeCost))
                {
                    currentCityHallLevel = nextLevel.level;
                    OnGridChanged?.Invoke();
                    return true;
                }
            }
            return false;
        }
    }
}
