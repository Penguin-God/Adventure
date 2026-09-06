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
    
    public static float GetFactoryAmmoPerSecond(BuildingModel factory, IEnumerable<BuildingModel> allBuildings)
    {
        var ammoBuffs = allBuildings
            .Where(building => building.data.buildingType == BuildingType.FactorySpeedBuff)
            .Where(building => IsInRange(factory.x, factory.y, building.x, building.y, building.data.buffRange))
            .Sum(building => building.data.buffAmount);
            
        return factory.data.ammoProductionTime + ammoBuffs; // using ammoProductionTime as base ammo per second
    }
    
    public static IEnumerable<BuildingModel> GetValidTowersForFactory(BuildingModel factory, IEnumerable<BuildingModel> allBuildings)
    {
        var roads = allBuildings.Where(building => building.data.buildingType == BuildingType.Road).ToList();
        var networkNodes = new HashSet<BuildingModel>();
        var queue = new Queue<BuildingModel>();
        
        queue.Enqueue(factory);
        networkNodes.Add(factory);
        
        while (queue.Count > 0)
        {
            var currentBuilding = queue.Dequeue();
            int currentRange = currentBuilding.data.buildingType == BuildingType.Factory ? currentBuilding.data.connectionRange : currentBuilding.data.buffRange;
            
            foreach (var road in roads)
            {
                if (!networkNodes.Contains(road) && IsInRange(currentBuilding.x, currentBuilding.y, road.x, road.y, currentRange))
                {
                    networkNodes.Add(road);
                    queue.Enqueue(road);
                }
            }
        }
        
        var towers = allBuildings.Where(b => b.data.buildingType == BuildingType.Tower).ToList();
        var validTowers = new HashSet<BuildingModel>();
        
        foreach (var node in networkNodes)
        {
            int nodeRange = node.data.buildingType == BuildingType.Factory ? node.data.connectionRange : node.data.buffRange;
            foreach (var tower in towers)
            {
                if (IsInRange(node.x, node.y, tower.x, tower.y, nodeRange))
                {
                    validTowers.Add(tower);
                }
            }
        }
        
        return validTowers;
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
