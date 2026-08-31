using UnityEngine;
using GatchTycoon.Domain;
using System.Collections.Generic;

namespace GatchTycoon.Data
{
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "GatchTycoon/BuildingData")]
    public class BuildingDataSO : ScriptableObject
    {
        public string buildingName;
        public BuildingCategory category;
        public int level;
        public GameObject prefab;
        
        public int capacity; 
        public int goldPerHour;
        public float baseOccupancyRate;
        public int buildCost;
        public CurrencyType costCurrency;
        
        public int buffRange;
        public float occupancyBuffAmount;
        public float goldEfficiencyBuffAmount;
        
        public BuildingDataSO nextLevelBuilding;
        
        public List<BuildingDataSO> combinationMaterials;
        public BuildingDataSO combinationResult;
    }
}
