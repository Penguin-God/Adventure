using UnityEngine;
using System.Linq;

public class DefenseManager : MonoBehaviour
{
    public static DefenseManager Instance { get; private set; }
    
    public int currentGold = 1000;
    private bool _isGameOver = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        MonsterManager.Instance.OnMonsterReachedEnd += HandleGameOver;
        MonsterManager.Instance.OnMonsterKilled += AddGold;
        
        StartCoroutine(SpawnMonstersRoutine());
    }
    
    private System.Collections.IEnumerator SpawnMonstersRoutine()
    {
        while (!_isGameOver)
        {
            yield return new WaitForSeconds(3f); 
            var monsterData = Resources.Load<MonsterDataSO>("Monsters/BasicMonster");
            if (monsterData != null)
            {
                MonsterManager.Instance.SpawnMonster(monsterData);
            }
        }
    }
    
    void Update()
    {
        if (_isGameOver) return;
        if (GridManager.Instance == null) return;
        
        var buildings = GridManager.Instance.GetAllBuildings();
        
        foreach (var building in buildings)
        {
            if (building.data.buildingType == BuildingType.Factory)
            {
                float productionSpeed = GridDomainLogic.GetFactorySpeed(building, buildings);
                building.productionTimer += Time.deltaTime;
                if (building.productionTimer >= productionSpeed)
                {
                    building.productionTimer = 0f;
                    DistributeAmmo(building, buildings);
                }
            }
            else if (building.data.buildingType == BuildingType.Tower)
            {
                if (building.currentAmmo > 0)
                {
                    building.attackTimer += Time.deltaTime;
                    if (building.attackTimer >= building.data.attackSpeed)
                    {
                        var targetMonster = GridDomainLogic.GetClosestMonster(building, MonsterManager.Instance.GetActiveMonsters());
                        if (targetMonster != null)
                        {
                            building.attackTimer = 0f;
                            building.currentAmmo--;
                            float damage = GridDomainLogic.GetTowerAttackDamage(building, buildings);
                            
                            // Fire projectile instead of instant damage
                            ProjectileManager.Instance.FireProjectile(new Vector3(building.x, building.y, 0), targetMonster.id, damage, 10f); // Speed 10
                        }
                    }
                }
            }
        }
    }
    
    private void DistributeAmmo(BuildingModel factory, System.Collections.Generic.IEnumerable<BuildingModel> buildings)
    {
        var towers = buildings.Where(building => building.data.buildingType == BuildingType.Tower).ToList();
        foreach (var tower in towers)
        {
            if (tower.currentAmmo < tower.data.maxAmmo && GridDomainLogic.IsConnected(factory, tower, buildings))
            {
                tower.currentAmmo++;
            }
        }
    }
    
    private void HandleGameOver()
    {
        _isGameOver = true;
        Debug.Log("Game Over! A monster reached the end.");
    }
    
    public void AddGold(int amount)
    {
        currentGold += amount;
    }
    
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }
        return false;
    }
}
