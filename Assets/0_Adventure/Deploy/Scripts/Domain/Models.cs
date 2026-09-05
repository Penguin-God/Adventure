using UnityEngine;

public class BuildingModel
{
    public string id;
    public int x;
    public int y;
    public BuildingDataSO data;
    
    public int currentAmmo;
    public float attackTimer;
    public float productionTimer;
    
    public BuildingModel(string id, int x, int y, BuildingDataSO data)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.data = data;
    }
}

public class MonsterModel
{
    public string id;
    public MonsterDataSO data;
    public float currentHp;
    public Vector3 currentPosition;
    public int currentPathIndex;
    
    public MonsterModel(string id, MonsterDataSO data, Vector3 startPosition)
    {
        this.id = id;
        this.data = data;
        this.currentHp = data.maxHp;
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
