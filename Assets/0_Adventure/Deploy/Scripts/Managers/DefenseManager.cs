using UnityEngine;
using System.Linq;

public class DefenseManager : MonoBehaviour
{
    public static DefenseManager Instance { get; private set; }
    
    public int currentGold;
    private bool _isGameOver = false;
    private GameSettingsSO _settings;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        _settings = Resources.Load<GameSettingsSO>("GameSettings");
        if (_settings != null) currentGold = _settings.startingGold;
        else currentGold = 3000;
        
        MonsterManager.Instance.OnMonsterReachedEnd += HandleGameOver;
        MonsterManager.Instance.OnMonsterKilled += AddGold;
        
        StartCoroutine(SpawnMonstersRoutine());
    }
    
    private System.Collections.IEnumerator SpawnMonstersRoutine()
    {
        float waitTime = _settings != null ? _settings.initialWaitTime : 30f;
        yield return new WaitForSeconds(waitTime);
        
        int spawnedCount = 0;
        
        while (!_isGameOver)
        {
            var monsterData = Resources.Load<MonsterDataSO>("Monsters/BasicMonster");
            if (monsterData != null)
            {
                int step = _settings != null ? _settings.monsterHpIncreaseStep : 10;
                float percent = _settings != null ? _settings.monsterHpIncreasePercent : 0.1f;
                float hpMultiplier = 1f + (spawnedCount / step) * percent;
                
                MonsterManager.Instance.SpawnMonster(monsterData, hpMultiplier);
                spawnedCount++;
            }
            float delay = _settings != null ? _settings.monsterSpawnDelay : 1f;
            yield return new WaitForSeconds(delay);
        }
    }
    
    void Update()
    {
        if (_isGameOver) return;
        
        var allBuildings = GridManager.Instance.GetAllBuildings();
        var factories = allBuildings.Where(b => b.data.buildingType == BuildingType.Factory).ToList();
        var towers = allBuildings.Where(b => b.data.buildingType == BuildingType.Tower).ToList();
        var monsters = MonsterManager.Instance.GetActiveMonsters().ToList();
        
        // 공장 총알 생산 및 분배
        foreach (var factory in factories)
        {
            factory.productionTimer += Time.deltaTime;
            if (factory.productionTimer >= 1f)
            {
                factory.productionTimer -= 1f;
                int amountToProduce = Mathf.RoundToInt(GridDomainLogic.GetFactoryAmmoPerSecond(factory, allBuildings));
                
                var validTowers = GridDomainLogic.GetValidTowersForFactory(factory, allBuildings).ToList();
                
                for (int i = 0; i < amountToProduce; i++)
                {
                    // 장전되지 않은 타워 중 현재 총알이 제일 적은 타워 찾기
                    var needyTowers = validTowers.Where(t => t.currentAmmo < t.data.maxAmmo).ToList();
                    if (needyTowers.Count == 0) break; // 모두 풀장전이면 더이상 분배 안함
                    
                    var targetTower = needyTowers.OrderBy(t => t.currentAmmo)
                                                 .ThenBy(t => Vector2.Distance(new Vector2(factory.x, factory.y), new Vector2(t.x, t.y)))
                                                 .First();
                                                 
                    targetTower.currentAmmo++;
                }
            }
        }
        
        // 타워 공격 로직
        foreach (var tower in towers)
        {
            if (tower.currentAmmo >= tower.data.maxAmmo && tower.isReloading)
            {
                tower.isReloading = false;
            }
            
            if (tower.currentAmmo <= 0)
            {
                tower.isReloading = true;
            }
            
            if (tower.isReloading) continue;
            
            tower.attackTimer += Time.deltaTime;
            if (tower.attackTimer >= tower.data.attackSpeed)
            {
                var targetMonster = GridDomainLogic.GetClosestMonster(tower, monsters);
                if (targetMonster != null && tower.currentAmmo > 0)
                {
                    tower.attackTimer = 0f;
                    tower.currentAmmo--;
                    
                    float damage = GridDomainLogic.GetTowerAttackDamage(tower, allBuildings);
                    ProjectileManager.Instance.FireProjectile(new Vector3(tower.x, tower.y, 0), targetMonster.id, damage, 10f);
                }
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
