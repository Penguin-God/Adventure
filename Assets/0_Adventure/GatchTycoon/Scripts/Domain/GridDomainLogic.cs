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
                .Where(b => b.data.category == BuildingCategory.Public && b.data.buffType == BuffType.OccupancyRate)
                .Where(b => IsInPattern(b.x, b.y, residence.x, residence.y, b.data.effectPattern))
                .Sum(b => b.data.buffAmount);
            return System.Math.Min(1.0f, rate + buffs);
        }
        
        public static float CalculateGoldEfficiency(BuildingModel building, IEnumerable<BuildingModel> allBuildings)
        {
            float eff = 1.0f;
            var buffs = allBuildings
                .Where(b => b.data.category == BuildingCategory.Convenience && b.data.buffType == BuffType.MoneyEfficiency)
                .Where(b => IsInPattern(b.x, b.y, building.x, building.y, b.data.effectPattern))
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
                
                var worksInRange = workBuildings.Where(w => IsInPattern(r.x, r.y, w.x, w.y, r.data.commutePattern)).ToList();
                
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
        
        public static bool IsInPattern(int originX, int originY, int targetX, int targetY, RangePattern pattern)
        {
            int dx = targetX - originX;
            int dy = targetY - originY;
            if (dx == 0 && dy == 0) return false;
            
            switch (pattern)
            {
                case RangePattern.LeftRight:
                    return dy == 0 && System.Math.Abs(dx) == 1;
                case RangePattern.Cross:
                    return (System.Math.Abs(dx) == 1 && dy == 0) || (System.Math.Abs(dy) == 1 && dx == 0);
                case RangePattern.Square3x3:
                    return System.Math.Abs(dx) <= 1 && System.Math.Abs(dy) <= 1;
                case RangePattern.TopDiagonals:
                    return dy == 1 && System.Math.Abs(dx) == 1;
                case RangePattern.AllDiagonals:
                    return System.Math.Abs(dx) == 1 && System.Math.Abs(dy) == 1;
                default:
                    return false;
            }
        }
    }
}
