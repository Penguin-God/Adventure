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
        
        public int baseGoldPerHour;
        public int buildCost;
        public CurrencyType costCurrency;
        
        // Residence specifics
        public int capacity; 
        public float baseOccupancyRate;
        public RangePattern commutePattern;
        
        // Work specifics
        public int totalJobs;
        public int profitPerWorker;
        
        // Convenience & Public specifics
        public RangePattern effectPattern;
        public BuffType buffType;
        public float buffAmount;
        
        // Merge
        public BuildingDataSO nextLevelBuilding;
        public int requiredCount = 3;
        public int combineCost;
    }
}
