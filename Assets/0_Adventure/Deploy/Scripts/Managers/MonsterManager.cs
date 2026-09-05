using System.Collections.Generic;
using UnityEngine;
using System;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }
    
    private List<MonsterModel> _activeMonsters = new List<MonsterModel>();
    private Dictionary<string, GameObject> _monsterObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Transform> _monsterHpBars = new Dictionary<string, Transform>();
    
    public Transform monstersParent;
    
    private List<Vector3> _pathPositions;
    
    public event Action OnMonsterReachedEnd;
    public event Action<int> OnMonsterKilled;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        GeneratePath();
    }
    
    private void GeneratePath()
    {
        _pathPositions = new List<Vector3>();
        float minBound = -1f;
        float maxBound = 10f;
        
        // Bottom edge: (-1,-1) to (10,-1)
        for (float xPos = minBound; xPos <= maxBound; xPos++) _pathPositions.Add(new Vector3(xPos, minBound, 0));
        // Right edge: (10, 0) to (10, 10)
        for (float yPos = 0; yPos <= maxBound; yPos++) _pathPositions.Add(new Vector3(maxBound, yPos, 0));
        // Top edge: (9, 10) to (-1, 10)
        for (float xPos = maxBound - 1; xPos >= minBound; xPos--) _pathPositions.Add(new Vector3(xPos, maxBound, 0));
        // Left edge: (-1, 9) to (-1, 0)
        for (float yPos = maxBound - 1; yPos >= 0; yPos--) _pathPositions.Add(new Vector3(minBound, yPos, 0));
    }
    
    public IEnumerable<MonsterModel> GetActiveMonsters() => _activeMonsters;
    
    public void SpawnMonster(MonsterDataSO monsterData)
    {
        string newId = Guid.NewGuid().ToString();
        var monsterModel = new MonsterModel(newId, monsterData, _pathPositions[0]);
        _activeMonsters.Add(monsterModel);
        
        var monsterObject = new GameObject($"Monster_{newId}");
        if (monstersParent != null) monsterObject.transform.SetParent(monstersParent);
        monsterObject.transform.position = _pathPositions[0];
        
        var spriteRenderer = monsterObject.AddComponent<SpriteRenderer>();
        if (monsterData.sprite != null) spriteRenderer.sprite = monsterData.sprite;
        else spriteRenderer.sprite = CreateBoxSprite(Color.red);
        spriteRenderer.sortingOrder = 10;
        
        // HP Bar Background
        var hpBg = new GameObject("HpBg");
        hpBg.transform.SetParent(monsterObject.transform);
        hpBg.transform.localPosition = new Vector3(0, 0.6f, 0);
        var bgSr = hpBg.AddComponent<SpriteRenderer>();
        bgSr.sprite = CreateBoxSprite(Color.black);
        hpBg.transform.localScale = new Vector3(1f, 0.2f, 1f);
        bgSr.sortingOrder = 11;
        
        // HP Bar Foreground
        var hpFg = new GameObject("HpFg");
        hpFg.transform.SetParent(hpBg.transform);
        hpFg.transform.localPosition = new Vector3(0, 0, 0); // Center aligned
        var fgSr = hpFg.AddComponent<SpriteRenderer>();
        fgSr.sprite = CreateBoxSprite(Color.green);
        fgSr.sortingOrder = 12;
        
        _monsterHpBars[newId] = hpFg.transform;
        _monsterObjects[newId] = monsterObject;
    }
    
    public void TakeDamage(string monsterId, float damageAmount)
    {
        var monsterModel = _activeMonsters.Find(monster => monster.id == monsterId);
        if (monsterModel == null) return;
        
        monsterModel.currentHp -= damageAmount;
        
        if (_monsterHpBars.ContainsKey(monsterId))
        {
            float hpRatio = Mathf.Clamp01(monsterModel.currentHp / monsterModel.data.maxHp);
            _monsterHpBars[monsterId].localScale = new Vector3(hpRatio, 1f, 1f);
        }
        
        if (monsterModel.currentHp <= 0)
        {
            OnMonsterKilled?.Invoke(monsterModel.data.rewardGold);
            RemoveMonster(monsterModel);
        }
    }
    
    private void RemoveMonster(MonsterModel monsterModel)
    {
        _activeMonsters.Remove(monsterModel);
        if (_monsterObjects.ContainsKey(monsterModel.id))
        {
            Destroy(_monsterObjects[monsterModel.id]);
            _monsterObjects.Remove(monsterModel.id);
            _monsterHpBars.Remove(monsterModel.id);
        }
    }
    
    void Update()
    {
        for (int index = _activeMonsters.Count - 1; index >= 0; index--)
        {
            var monsterModel = _activeMonsters[index];
            var targetPosition = _pathPositions[Mathf.Min(monsterModel.currentPathIndex + 1, _pathPositions.Count - 1)];
            
            float distanceToTarget = Vector3.Distance(monsterModel.currentPosition, targetPosition);
            float moveStep = monsterModel.data.speed * Time.deltaTime;
            
            if (moveStep >= distanceToTarget)
            {
                monsterModel.currentPosition = targetPosition;
                monsterModel.currentPathIndex++;
                
                if (monsterModel.currentPathIndex >= _pathPositions.Count - 1)
                {
                    OnMonsterReachedEnd?.Invoke();
                    RemoveMonster(monsterModel);
                    continue;
                }
            }
            else
            {
                monsterModel.currentPosition = Vector3.MoveTowards(monsterModel.currentPosition, targetPosition, moveStep);
            }
            
            if (_monsterObjects.ContainsKey(monsterModel.id))
            {
                _monsterObjects[monsterModel.id].transform.position = monsterModel.currentPosition;
            }
        }
    }
    
    private Sprite CreateBoxSprite(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
