using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridDomainLogic
{
    public static bool IsInRange(int startX, int startY, int targetX, int targetY, int range) =>
        Mathf.Abs(startX - targetX) <= range && Mathf.Abs(startY - targetY) <= range;
        
    public static float GetTowerAttackDamage(BuildingModel tower, IEnumerable<BuildingModel> allBuildings)
    {
        var attackBuffs = allBuildings
            .Where(building => building.data.buildingType == BuildingType.TowerAttackBuff)
            .Where(building => IsInRange(tower.x, tower.y, building.x, building.y, building.data.buffRange))
            .Sum(building => building.data.buffAmount);
            
        return tower.data.attackDamage + attackBuffs;
    }
    
    public static float GetFactorySpeed(BuildingModel factory, IEnumerable<BuildingModel> allBuildings)
    {
        var speedBuffs = allBuildings
            .Where(building => building.data.buildingType == BuildingType.FactorySpeedBuff)
            .Where(building => IsInRange(factory.x, factory.y, building.x, building.y, building.data.buffRange))
            .Sum(building => building.data.buffAmount);
            
        return Mathf.Max(0.1f, factory.data.ammoProductionTime - speedBuffs);
    }
    
    public static bool IsConnected(BuildingModel source, BuildingModel target, IEnumerable<BuildingModel> allBuildings)
    {
        var roads = allBuildings.Where(building => building.data.buildingType == BuildingType.Road).ToList();
        var visitedIds = new HashSet<string>();
        var queue = new Queue<BuildingModel>();
        
        queue.Enqueue(source);
        visitedIds.Add(source.id);
        
        while (queue.Count > 0)
        {
            var currentBuilding = queue.Dequeue();
            int currentRange = currentBuilding.data.buildingType == BuildingType.Factory ? currentBuilding.data.connectionRange : 
                               (currentBuilding.data.buildingType == BuildingType.Road ? currentBuilding.data.buffRange : 1);
            
            if (IsInRange(currentBuilding.x, currentBuilding.y, target.x, target.y, currentRange)) return true;
            
            foreach (var road in roads)
            {
                if (!visitedIds.Contains(road.id) && IsInRange(currentBuilding.x, currentBuilding.y, road.x, road.y, currentRange))
                {
                    visitedIds.Add(road.id);
                    queue.Enqueue(road);
                }
            }
        }
        
        return false;
    }
    
    public static MonsterModel GetClosestMonster(BuildingModel tower, IEnumerable<MonsterModel> monsters)
    {
        MonsterModel closestMonster = null;
        float minDistance = float.MaxValue;
        
        Vector2 towerPosition = new Vector2(tower.x, tower.y);
        
        foreach (var monster in monsters)
        {
            Vector2 monsterPosition = new Vector2(monster.currentPosition.x, monster.currentPosition.y);
            float distance = Vector2.Distance(towerPosition, monsterPosition);
            
            if (distance <= tower.data.attackRange && distance < minDistance)
            {
                minDistance = distance;
                closestMonster = monster;
            }
        }
        
        return closestMonster;
    }
}
