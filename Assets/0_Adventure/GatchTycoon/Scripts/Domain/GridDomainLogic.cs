using GatchTycoon.Data;
using System.Collections.Generic;
using System.Linq;

namespace GatchTycoon.Domain
{
    public class BuildingModel
    {
        public string id;
        public BuildingDataSO data;
        public int x;
        public int y;
        
        public BuildingModel(string id, BuildingDataSO data, int x, int y)
        {
            this.id = id;
            this.data = data;
            this.x = x;
            this.y = y;
        }
    }

    public static class GridDomainLogic
    {
        public static float CalculateOccupancyRate(BuildingModel residence, IEnumerable<BuildingModel> allBuildings)
        {
            float rate = residence.data.baseOccupancyRate;
            var buffs = allBuildings
                .Where(b => b.data.category == BuildingCategory.Mixed)
                .Where(b => IsInRange(residence.x, residence.y, b.x, b.y, b.data.buffRange))
                .Sum(b => b.data.occupancyBuffAmount);
            return rate + buffs;
        }
        
        public static float CalculateGoldEfficiency(BuildingModel corporation, IEnumerable<BuildingModel> allBuildings)
        {
            float eff = 1.0f;
            var buffs = allBuildings
                .Where(b => b.data.category == BuildingCategory.Mixed)
                .Where(b => IsInRange(corporation.x, corporation.y, b.x, b.y, b.data.buffRange))
                .Sum(b => b.data.goldEfficiencyBuffAmount);
            return eff + buffs;
        }
        
        public static int CalculateCorporationGold(BuildingModel corporation, IEnumerable<BuildingModel> allBuildings)
        {
            int totalResidents = allBuildings
                .Where(b => b.data.category == BuildingCategory.Residence)
                .Sum(b => (int)(b.data.capacity * CalculateOccupancyRate(b, allBuildings)));
            
            int activeWorkers = System.Math.Min(totalResidents, corporation.data.capacity);
            
            float eff = CalculateGoldEfficiency(corporation, allBuildings);
            
            return (int)(activeWorkers * corporation.data.goldPerHour * eff);
        }
        
        public static bool IsInRange(int x1, int y1, int x2, int y2, int range) =>
            System.Math.Abs(x1 - x2) <= range && System.Math.Abs(y1 - y2) <= range;
    }
}
