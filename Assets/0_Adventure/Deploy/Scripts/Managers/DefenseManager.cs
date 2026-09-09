using UnityEngine;
using System.Linq;

public class DefenseManager : MonoBehaviour
{
    public static DefenseManager Instance { get; private set; }
    
    public int currentGold;
    public float elapsedTime = 0f;
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
        
        elapsedTime += Time.deltaTime;
        
        var allBuildings = GridManager.Instance.GetAllBuildings();
        var towers = allBuildings.Where(b => b.data.buildingType == BuildingType.Tower).ToList();
        var monsters = MonsterManager.Instance.GetActiveMonsters().ToList();
        
        // 타워 공격 로직
        foreach (var tower in towers)
        {
            if (tower.currentInput1 >= tower.data.maxAmmo && tower.isShutdown)
            {
                tower.isShutdown = false;
            }
            
            if (tower.currentInput1 <= 0)
            {
                tower.isShutdown = true;
            }
            
            if (tower.isShutdown) continue;
            
            tower.attackTimer += Time.deltaTime;
            if (tower.attackTimer >= tower.data.attackSpeed)
            {
                var targetMonster = GridDomainLogic.GetClosestMonster(tower, monsters);
                if (targetMonster != null && tower.currentInput1 > 0)
                {
                    tower.attackTimer = 0f;
                    tower.currentInput1--;
                    
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
