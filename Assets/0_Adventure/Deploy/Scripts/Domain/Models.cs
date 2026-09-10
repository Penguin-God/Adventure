using UnityEngine;

// Enums are moved to Enums.cs

public class BuildingModel
{
    public string id;
    public int x;
    public int y;
    public BuildingDataSO data;
    
    // Inventory
    public int currentInput1;
    public int currentInput2;
    public int currentOutput;
    
    // Logic Timers
    public float attackTimer;
    public float productionTimer;
    public float mineTimer;
    
    // Shutdown state
    public bool isShutdown;
    
    public BuildingModel(string id, int x, int y, BuildingDataSO data)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.data = data;
        
        this.currentInput1 = 0;
        this.currentInput2 = 0;
        this.currentOutput = 0;
        
        // Start as shutdown since inputs/outputs are 0
        this.isShutdown = true; 
    }
}

public class MonsterModel
{
    public string id;
    public MonsterDataSO data;
    public float maxHp;
    public float currentHp;
    public Vector3 currentPosition;
    public int currentPathIndex;
    public float speed;
    
    public MonsterModel(string id, MonsterDataSO data, Vector3 startPosition, float hpMultiplier = 1f, float speedOverride = -1f)
    {
        this.id = id;
        this.data = data;
        this.maxHp = data.maxHp * hpMultiplier;
        this.currentHp = this.maxHp;
        this.speed = speedOverride > 0 ? speedOverride : data.speed;
        this.currentPosition = startPosition;
        this.currentPathIndex = 0;
    }
}

public class ProjectileModel
{
    public string id;
    public Vector3 currentPosition;
    public string targetMonsterId;
    public float damage;
    public float speed;
    
    public ProjectileModel(string id, Vector3 startPosition, string targetMonsterId, float damage, float speed)
    {
        this.id = id;
        this.currentPosition = startPosition;
        this.targetMonsterId = targetMonsterId;
        this.damage = damage;
        this.speed = speed;
    }
}
