using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance { get; private set; }
    
    private List<ProjectileModel> _activeProjectiles = new List<ProjectileModel>();
    private Dictionary<string, GameObject> _projectileObjects = new Dictionary<string, GameObject>();
    
    public Transform projectilesParent;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void FireProjectile(Vector3 startPosition, string targetMonsterId, float damage, float speed)
    {
        string newId = Guid.NewGuid().ToString();
        var projectileModel = new ProjectileModel(newId, startPosition, targetMonsterId, damage, speed);
        _activeProjectiles.Add(projectileModel);
        
        var projectileObject = new GameObject($"Projectile_{newId}");
        if (projectilesParent != null) projectileObject.transform.SetParent(projectilesParent);
        projectileObject.transform.position = startPosition;
        
        var spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = Color.yellow;
        projectileObject.transform.localScale = new Vector3(0.05f, 0.05f, 1f);
        spriteRenderer.sortingOrder = 15;
        
        _projectileObjects[newId] = projectileObject;
    }
    
    void Update()
    {
        for (int index = _activeProjectiles.Count - 1; index >= 0; index--)
        {
            var projectileModel = _activeProjectiles[index];
            var activeMonsters = MonsterManager.Instance.GetActiveMonsters();
            var targetMonster = System.Linq.Enumerable.FirstOrDefault(activeMonsters, monster => monster.id == projectileModel.targetMonsterId);
            
            if (targetMonster == null)
            {
                // Target is dead or gone, destroy projectile
                RemoveProjectile(projectileModel);
                continue;
            }
            
            float distanceToTarget = Vector3.Distance(projectileModel.currentPosition, targetMonster.currentPosition);
            float moveStep = projectileModel.speed * Time.deltaTime;
            
            if (moveStep >= distanceToTarget)
            {
                MonsterManager.Instance.TakeDamage(targetMonster.id, projectileModel.damage);
                RemoveProjectile(projectileModel);
                continue;
            }
            else
            {
                projectileModel.currentPosition = Vector3.MoveTowards(projectileModel.currentPosition, targetMonster.currentPosition, moveStep);
            }
            
            if (_projectileObjects.ContainsKey(projectileModel.id))
            {
                _projectileObjects[projectileModel.id].transform.position = projectileModel.currentPosition;
            }
        }
    }
    
    private void RemoveProjectile(ProjectileModel projectileModel)
    {
        _activeProjectiles.Remove(projectileModel);
        if (_projectileObjects.ContainsKey(projectileModel.id))
        {
            Destroy(_projectileObjects[projectileModel.id]);
            _projectileObjects.Remove(projectileModel.id);
        }
    }
    
    private Sprite CreateCircleSprite()
    {
        // Simple 16x16 circle texture
        int resolution = 16;
        Texture2D texture = new Texture2D(resolution, resolution);
        Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
        float radius = resolution / 2f;
        
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float distance = Vector2.Distance(center, new Vector2(x, y));
                if (distance <= radius) texture.SetPixel(x, y, Color.white);
                else texture.SetPixel(x, y, Color.clear);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 1f);
    }
}
