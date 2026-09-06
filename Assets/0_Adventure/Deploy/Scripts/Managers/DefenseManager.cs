using UnityEngine;
using System.Linq;

public class DefenseManager : MonoBehaviour
{
    public static DefenseManager Instance { get; private set; }
    
    public int currentGold = 3000;
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
        
        // 초기 타워 배치
        var cannon = Resources.Load<BuildingDataSO>("Buildings/Cannon");
        var archer = Resources.Load<BuildingDataSO>("Buildings/Archer");
        if (cannon != null) GridManager.Instance.PlaceBuilding(cannon, 0, 0);
        if (archer != null) GridManager.Instance.PlaceBuilding(archer, 0, 9);
        if (archer != null) GridManager.Instance.PlaceBuilding(archer, 9, 0);
        
        StartCoroutine(SpawnMonstersRoutine());
    }
    
    private System.Collections.IEnumerator SpawnMonstersRoutine()
    {
        yield return new WaitForSeconds(10f);
        
        while (!_isGameOver)
        {
            var monsterData = Resources.Load<MonsterDataSO>("Monsters/BasicMonster");
            if (monsterData != null)
            {
                MonsterManager.Instance.SpawnMonster(monsterData);
            }
            yield return new WaitForSeconds(1f); 
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
