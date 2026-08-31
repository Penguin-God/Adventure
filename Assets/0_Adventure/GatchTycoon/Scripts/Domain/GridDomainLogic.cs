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
            float rate = 0.5f; // Base occupancy
            var buffs = allBuildings
                .Where(b => b.data.category == BuildingCategory.Convenience && b.data.buffType == BuffType.OccupancyRate)
                .Where(b => IsInRange(residence.x, residence.y, b.x, b.y, b.data.buffRange))
                .Sum(b => b.data.buffAmount);
            return rate + buffs;
        }
        
        public static float CalculateGoldEfficiency(BuildingModel building, IEnumerable<BuildingModel> allBuildings)
        {
            float eff = 1.0f;
            var buffs = allBuildings
                .Where(b => b.data.category == BuildingCategory.Convenience && b.data.buffType == BuffType.MoneyEfficiency)
                .Where(b => IsInRange(building.x, building.y, b.x, b.y, b.data.buffRange))
                .Sum(b => b.data.buffAmount);
            return eff + buffs;
        }
        
        public static Dictionary<string, int> CalculateWorkerAssignments(IEnumerable<BuildingModel> allBuildings)
        {
            var assignments = new Dictionary<string, int>();
            var workBuildings = allBuildings.Where(b => b.data.category == BuildingCategory.Work).ToList();
            var residenceBuildings = allBuildings.Where(b => b.data.category == BuildingCategory.Residence).ToList();
            
            foreach (var w in workBuildings) assignments[w.id] = 0;
            
            foreach (var r in residenceBuildings)
            {
                float occupancyRate = CalculateOccupancyRate(r, allBuildings);
                int availableWorkers = (int)(r.data.capacity * occupancyRate);
                
                var worksInRange = workBuildings.Where(w => IsInRange(r.x, r.y, w.x, w.y, r.data.commuteRange)).ToList();
                
                foreach (var w in worksInRange)
                {
                    if (availableWorkers <= 0) break;
                    
                    int currentWorkers = assignments[w.id];
                    int availableJobs = w.data.totalJobs - currentWorkers;
                    
                    if (availableJobs > 0)
                    {
                        int toAssign = System.Math.Min(availableWorkers, availableJobs);
                        assignments[w.id] += toAssign;
                        availableWorkers -= toAssign;
                    }
                }
            }
            return assignments;
        }
        
        public static int CalculateBuildingGold(BuildingModel b, IEnumerable<BuildingModel> allBuildings, Dictionary<string, int> assignments)
        {
            float eff = CalculateGoldEfficiency(b, allBuildings);
            int baseGold = (int)(b.data.baseGoldPerHour * eff);
            
            if (b.data.category == BuildingCategory.Work && assignments.ContainsKey(b.id))
            {
                int workers = assignments[b.id];
                return baseGold + (int)(workers * b.data.profitPerWorker * eff);
            }
            
            return baseGold;
        }
        
        public static bool IsInRange(int x1, int y1, int x2, int y2, int range) =>
            System.Math.Abs(x1 - x2) <= range && System.Math.Abs(y1 - y2) <= range;
    }
}
