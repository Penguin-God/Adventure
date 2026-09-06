using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : MonoBehaviour
{
    public Transform tilesParent;
    public Transform buildingsParent;
    
    private Dictionary<string, GameObject> _buildingObjects = new Dictionary<string, GameObject>();
    
    void Start()
    {
        GridManager.Instance.OnGridChanged += DrawGrid;
        GridManager.Instance.OnBuildingPlaced += OnBuildingPlaced;
        DrawGrid();
    }
    
    private void DrawGrid()
    {
        if (tilesParent.childCount == 0)
        {
            for (int posX = 0; posX < 10; posX++)
            {
                for (int posY = 0; posY < 10; posY++)
                {
                    var tileGo = new GameObject($"Tile_{posX}_{posY}");
                    tileGo.transform.position = new Vector3(posX, posY, 0.1f); 
                    tileGo.transform.SetParent(tilesParent);
                    var sr = tileGo.AddComponent<SpriteRenderer>();
                    sr.sprite = CreateBoxSprite();
                    sr.color = new Color(0.8f, 0.8f, 0.8f);
                    tileGo.transform.localScale = new Vector3(0.95f, 0.95f, 1);
                }
            }
        }
        
        foreach (var building in GridManager.Instance.GetAllBuildings())
        {
            if (_buildingObjects.ContainsKey(building.id))
            {
                _buildingObjects[building.id].transform.position = new Vector3(building.x, building.y, 0);
            }
        }
    }
    
    void Update()
    {
        if (DeckManager.Instance == null) return;
        
        bool isPlacing = DeckManager.Instance.IsPlacing;
        
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            var tile = tilesParent.GetChild(i);
            var sr = tile.GetComponent<SpriteRenderer>();
            
            if (isPlacing)
            {
                int posX = Mathf.RoundToInt(tile.position.x);
                int posY = Mathf.RoundToInt(tile.position.y);
                
                if (DeckManager.Instance.IsValidPlacement(posX, posY))
                {
                    sr.color = new Color(0.8f, 1.0f, 0.8f); // 연한 녹색 (설치 가능)
                }
                else
                {
                    sr.color = new Color(1.0f, 0.6f, 0.6f); // 연한 빨간색 (설치 불가)
                }
            }
            else
            {
                sr.color = new Color(0.8f, 0.8f, 0.8f); // 기본 색
            }
        }
    }
    
    private void OnBuildingPlaced(BuildingModel model)
    {
        var buildingGo = new GameObject($"Building_{model.data.buildingName}");
        buildingGo.transform.position = new Vector3(model.x, model.y, 0);
        buildingGo.transform.SetParent(buildingsParent);
        
        var sr = buildingGo.AddComponent<SpriteRenderer>();
        if (model.data.sprite != null) sr.sprite = model.data.sprite;
        else sr.sprite = CreateBoxSprite();
        
        switch(model.data.buildingType)
        {
            case BuildingType.Factory: sr.color = Color.blue; break;
            case BuildingType.Road: sr.color = Color.gray; break;
            case BuildingType.Tower: sr.color = Color.red; break;
            case BuildingType.FactorySpeedBuff: sr.color = Color.cyan; break;
            case BuildingType.TowerAttackBuff: sr.color = Color.magenta; break;
        }
        
        buildingGo.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        _buildingObjects[model.id] = buildingGo;
    }
    
    private Sprite CreateBoxSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
