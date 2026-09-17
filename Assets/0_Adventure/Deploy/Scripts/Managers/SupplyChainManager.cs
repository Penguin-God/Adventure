using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupplyChainManager : MonoBehaviour
{
    public static SupplyChainManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Update()
    {
        var allBuildings = GridManager.Instance.GetAllBuildings().ToList();
        
        // 1. Process Mines
        foreach (var mine in allBuildings.Where(b => b.data.buildingType == BuildingType.Mine))
        {
            mine.mineTimer += Time.deltaTime;
            float productionTime = mine.data.productionTime > 0 ? mine.data.productionTime : 0.1f;
            
            if (mine.mineTimer >= productionTime)
            {
                mine.mineTimer = 0;
                if (mine.currentOutput < mine.data.maxOutputCapacity)
                {
                    mine.currentOutput++;
                }
            }
            
            if (mine.currentOutput <= 0) mine.isShutdown = true;
            else mine.isShutdown = false;
            
            if (!mine.isShutdown && mine.currentOutput > 0)
            {
                if (DistributeOutput(mine, mine.data.outputType, null, allBuildings)) mine.currentOutput--;
            }
        }
        
        // 2. Process Factories
        foreach (var factory in allBuildings.Where(b => b.data.buildingType == BuildingType.Factory))
        {
            bool hasInput1 = factory.data.inputType1 == ResourceType.None || factory.currentInput1 > 0;
            bool hasInput2 = factory.data.inputType2 == ResourceType.None || factory.currentInput2 > 0;
            
            if (!hasInput1 || !hasInput2) factory.isShutdown = true;
            else factory.isShutdown = false;
            
            if (!factory.isShutdown && factory.currentOutput < factory.data.maxOutputCapacity)
            {
                factory.productionTimer += Time.deltaTime;
                float prodTime = factory.data.productionTime > 0 ? factory.data.productionTime : 1f;
                float buff = GetSpeedBuff(factory, allBuildings);
                prodTime = Mathf.Max(0.1f, prodTime - buff - GridDomainLogic.GetUpgradedValue(factory));
                
                if (factory.productionTimer >= prodTime)
                {
                    factory.productionTimer = 0;
                    if (factory.data.inputType1 != ResourceType.None) factory.currentInput1--;
                    if (factory.data.inputType2 != ResourceType.None) factory.currentInput2--;
                    factory.currentOutput++;
                }
            }
            
            if (!factory.isShutdown && factory.currentOutput > 0)
            {
                if (DistributeOutput(factory, factory.data.outputType, null, allBuildings)) factory.currentOutput--;
            }
        }
        
        // 3. Process Smelteries
        foreach (var smeltery in allBuildings.Where(b => b.data.buildingType == BuildingType.Smeltery))
        {
            if (smeltery.selectedAmmoType == ResourceType.None)
            {
                smeltery.isShutdown = true;
                continue;
            }
            
            bool hasInput = smeltery.inputQueue.Count > 0;
            if (!hasInput) smeltery.isShutdown = true;
            else smeltery.isShutdown = false;
            
            if (!smeltery.isShutdown && smeltery.outputQueue.Count < smeltery.data.maxOutputCapacity)
            {
                smeltery.productionTimer += Time.deltaTime;
                float prodTime = smeltery.data.productionTime > 0 ? smeltery.data.productionTime : 1f;
                
                if (smeltery.productionTimer >= prodTime)
                {
                    smeltery.productionTimer = 0;
                    var item = smeltery.inputQueue.Dequeue();
                    smeltery.currentInput1--;
                    if (smeltery.data.buildingName == "SlowEffect") 
                    {
                        item.slowAmount += 0.5f + GridDomainLogic.GetUpgradedValue(smeltery);
                        item.slowAmount = Mathf.Min(0.9f, item.slowAmount);
                    }
                    if (smeltery.data.buildingName == "DamageEffect") 
                    {
                        item.bonusDamage += 50f + GridDomainLogic.GetUpgradedValue(smeltery);
                    }
                    smeltery.outputQueue.Enqueue(item);
                    smeltery.currentOutput++;
                }
            }
            
            if (smeltery.outputQueue.Count > 0)
            {
                var item = smeltery.outputQueue.Peek();
                if (DistributeOutput(smeltery, item.type, item, allBuildings))
                {
                    smeltery.outputQueue.Dequeue();
                    smeltery.currentOutput--;
                }
            }
        }
    }
    
    
    
    private bool DistributeOutput(BuildingModel source, ResourceType baseType, AmmoItem itemToDistribute, List<BuildingModel> allBuildings)
    {
        var validTargets = GetConnectedConsumers(source, baseType, allBuildings);
        
        if (validTargets.Count > 0)
        {
            var target = validTargets
                .OrderBy(b => GetCurrentNeedLevel(b, baseType))
                .ThenBy(b => Vector2.Distance(new Vector2(source.x, source.y), new Vector2(b.x, b.y)))
                .FirstOrDefault();
                
            if (target != null && GiveResource(target, baseType, itemToDistribute))
            {
                return true;
            }
        }
        return false;
    }
    
    private int GetCurrentNeedLevel(BuildingModel b, ResourceType type)
    {
        if (b.data.buildingType == BuildingType.Tower) return b.currentInput1;
        if (b.data.buildingType == BuildingType.Smeltery)
        {
            if (b.selectedAmmoType == type) return b.currentInput1;
        }
        if (b.data.buildingType == BuildingType.Factory)
        {
            if (b.data.inputType1 == type) return b.currentInput1;
            if (b.data.inputType2 == type) return b.currentInput2;
        }
        return int.MaxValue;
    }
    
    private bool GiveResource(BuildingModel target, ResourceType type, AmmoItem itemToDistribute)
    {
        if (target.data.buildingType == BuildingType.Tower)
        {
            if (target.currentInput1 < target.data.maxAmmo)
            {
                target.currentInput1++;
                target.inputQueue.Enqueue(itemToDistribute ?? new AmmoItem(type));
                return true;
            }
        }
        else if (target.data.buildingType == BuildingType.Smeltery)
        {
            if (target.selectedAmmoType == type && target.currentInput1 < target.data.maxInputCapacity)
            {
                target.currentInput1++;
                target.inputQueue.Enqueue(itemToDistribute ?? new AmmoItem(type));
                return true;
            }
        }
        else if (target.data.buildingType == BuildingType.Factory)
        {
            if (target.data.inputType1 == type && target.currentInput1 < target.data.maxInputCapacity)
            {
                target.currentInput1++;
                return true;
            }
            if (target.data.inputType2 == type && target.currentInput2 < target.data.maxInputCapacity)
            {
                target.currentInput2++;
                return true;
            }
        }

        else if (target.data.buildingType == BuildingType.Hall)
        {
            if (type == ResourceType.Wood) { GameState.currentWood++; return true; }
            if (type == ResourceType.Iron) { GameState.currentIron++; return true; }
            if (type == ResourceType.Hammer) { GameState.currentHammer++; return true; }
        }
        return false;
    }
    
    private List<BuildingModel> GetConnectedConsumers(BuildingModel source, ResourceType type, List<BuildingModel> allBuildings)
    {
        var conduits = allBuildings.Where(b => b.data.buildingType == BuildingType.Road || b.data.buildingType == BuildingType.Entrance).ToList();
        var networkNodes = new HashSet<BuildingModel>();
        var queue = new Queue<BuildingModel>();
        
        queue.Enqueue(source);
        networkNodes.Add(source);
        
        while (queue.Count > 0)
        {
            var currentBuilding = queue.Dequeue();
            int currentRange = currentBuilding.data.buildingType == BuildingType.Factory ? currentBuilding.data.connectionRange : 1;
            
            foreach (var node in conduits)
            {
                if (!networkNodes.Contains(node) && GridDomainLogic.IsInRange(currentBuilding.x, currentBuilding.y, node.x, node.y, currentRange))
                {
                    networkNodes.Add(node);
                    queue.Enqueue(node);
                }
            }
        }
        
        var validConsumers = new HashSet<BuildingModel>();
        
        foreach (var node in networkNodes)
        {
            int nodeRange = node.data.buildingType == BuildingType.Factory ? node.data.connectionRange : 1;
            
            foreach (var factory in allBuildings.Where(b => b.data.buildingType == BuildingType.Factory && b != source))
            {
                if (GridDomainLogic.IsInRange(node.x, node.y, factory.x, factory.y, nodeRange))
                {
                    if ((factory.data.inputType1 == type && factory.currentInput1 < factory.data.maxInputCapacity) ||
                        (factory.data.inputType2 == type && factory.currentInput2 < factory.data.maxInputCapacity))
                    {
                        validConsumers.Add(factory);
                    }
                }
            }
            
            foreach (var smeltery in allBuildings.Where(b => b.data.buildingType == BuildingType.Smeltery && b != source))
            {
                if (GridDomainLogic.IsInRange(node.x, node.y, smeltery.x, smeltery.y, nodeRange))
                {
                    if (smeltery.selectedAmmoType == type && smeltery.currentInput1 < smeltery.data.maxInputCapacity)
                    {
                        validConsumers.Add(smeltery);
                    }
                }
            }
            
            foreach (var tower in allBuildings.Where(b => b.data.buildingType == BuildingType.Tower))
            {
                if (GridDomainLogic.IsInRange(node.x, node.y, tower.x, tower.y, nodeRange))
                {
                    if (GridDomainLogic.IsMatchingAmmo(tower.data.requiredAmmoType, type) && tower.currentInput1 < tower.data.maxAmmo)
                    {
                        validConsumers.Add(tower);
                    }
                }
            }
            
            if (node.data.buildingType == BuildingType.Entrance)
            {
                int zoneXStart = node.x;
                int zoneXEnd = node.x;
                if (node.x == 0) { zoneXStart = 0; zoneXEnd = 1; }
                else if (node.x == 7) { zoneXStart = 7; zoneXEnd = 8; }
                else if (node.x == 14) { zoneXStart = 13; zoneXEnd = 14; }
                
                int zoneYStart = -4;
                int zoneYEnd = -2;
                
                foreach (var tower in allBuildings.Where(b => b.data.buildingType == BuildingType.Tower))
                {
                    if (tower.x >= zoneXStart && tower.x <= zoneXEnd && tower.y >= zoneYStart && tower.y <= zoneYEnd)
                    {
                        if (GridDomainLogic.IsMatchingAmmo(tower.data.requiredAmmoType, type) && tower.currentInput1 < tower.data.maxAmmo)
                        {
                            validConsumers.Add(tower);
                        }
                    }
                }
            }
            
            // Add Town Hall logic for Village resources
            if (type == ResourceType.Wood || type == ResourceType.Iron || type == ResourceType.Hammer)
            {
                foreach (var hall in allBuildings.Where(b => b.data.buildingType == BuildingType.Hall))
                {
                    if (GridDomainLogic.IsInRange(node.x, node.y, hall.x, hall.y, nodeRange))
                    {
                        validConsumers.Add(hall);
                    }
                }
            }
        }
        
        return validConsumers.Distinct().ToList();
    }
    
    private float GetSpeedBuff(BuildingModel factory, List<BuildingModel> allBuildings)
    {
        return allBuildings
            .Where(b => b.data.buildingType == BuildingType.FactorySpeedBuff)
            .Where(b => GridDomainLogic.IsInRange(factory.x, factory.y, b.x, b.y, b.data.buffRange))
            .Sum(b => b.data.buffAmount + GridDomainLogic.GetUpgradedValue(b));
    }
}
