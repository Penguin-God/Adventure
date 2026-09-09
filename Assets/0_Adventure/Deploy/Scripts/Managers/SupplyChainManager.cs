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
            else if (mine.currentOutput >= mine.data.maxOutputCapacity) mine.isShutdown = false;
            
            if (!mine.isShutdown && mine.currentOutput > 0)
            {
                DistributeOutput(mine, mine.data.outputType, allBuildings);
            }
        }
        
        // 2. Process Factories
        foreach (var factory in allBuildings.Where(b => b.data.buildingType == BuildingType.Factory))
        {
            if (factory.currentOutput <= 0) factory.isShutdown = true;
            else if (factory.currentOutput >= factory.data.maxOutputCapacity) factory.isShutdown = false;
            
            bool hasInput1 = factory.data.inputType1 == ResourceType.None || factory.currentInput1 > 0;
            bool hasInput2 = factory.data.inputType2 == ResourceType.None || factory.currentInput2 > 0;
            
            if (hasInput1 && hasInput2 && factory.currentOutput < factory.data.maxOutputCapacity)
            {
                factory.productionTimer += Time.deltaTime;
                float prodTime = factory.data.productionTime > 0 ? factory.data.productionTime : 1f;
                float buff = GetSpeedBuff(factory, allBuildings);
                prodTime = Mathf.Max(0.1f, prodTime - buff);
                
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
                DistributeOutput(factory, factory.data.outputType, allBuildings);
            }
        }
    }
    
    private void DistributeOutput(BuildingModel source, ResourceType outputType, List<BuildingModel> allBuildings)
    {
        var validTargets = GetConnectedConsumers(source, outputType, allBuildings);
        
        if (validTargets.Count > 0)
        {
            var target = validTargets
                .OrderBy(b => GetCurrentNeedLevel(b, outputType))
                .ThenBy(b => Vector2.Distance(new Vector2(source.x, source.y), new Vector2(b.x, b.y)))
                .FirstOrDefault();
                
            if (target != null && GiveResource(target, outputType))
            {
                source.currentOutput--;
            }
        }
    }
    
    private int GetCurrentNeedLevel(BuildingModel b, ResourceType type)
    {
        if (b.data.buildingType == BuildingType.Tower) return b.currentInput1;
        if (b.data.buildingType == BuildingType.Factory)
        {
            if (b.data.inputType1 == type) return b.currentInput1;
            if (b.data.inputType2 == type) return b.currentInput2;
        }
        return int.MaxValue;
    }
    
    private bool GiveResource(BuildingModel target, ResourceType type)
    {
        if (target.data.buildingType == BuildingType.Tower)
        {
            if (target.currentInput1 < target.data.maxAmmo)
            {
                target.currentInput1++;
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
        return false;
    }
    
    private List<BuildingModel> GetConnectedConsumers(BuildingModel source, ResourceType type, List<BuildingModel> allBuildings)
    {
        var conduits = allBuildings.Where(b => b.data.buildingType == BuildingType.Road || b.data.buildingType == BuildingType.Entrance || b.data.buildingType == BuildingType.Factory).ToList();
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
            
            foreach (var factory in allBuildings.Where(b => b.data.buildingType == BuildingType.Factory))
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
            
            if (node.data.buildingType == BuildingType.Entrance)
            {
                int zoneXStart = node.x;
                int zoneXEnd = node.x;
                if (node.x == 0) { zoneXStart = 0; zoneXEnd = 3; }
                else if (node.x == 7) { zoneXStart = 5; zoneXEnd = 8; }
                else if (node.x == 14) { zoneXStart = 11; zoneXEnd = 14; }
                
                int zoneYStart = -3;
                int zoneYEnd = -2;
                
                foreach (var tower in allBuildings.Where(b => b.data.buildingType == BuildingType.Tower))
                {
                    if (tower.x >= zoneXStart && tower.x <= zoneXEnd && tower.y >= zoneYStart && tower.y <= zoneYEnd)
                    {
                        if (tower.data.requiredAmmoType == type && tower.currentInput1 < tower.data.maxAmmo)
                        {
                            validConsumers.Add(tower);
                        }
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
            .Sum(b => b.data.buffAmount);
    }
}
