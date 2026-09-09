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
                
                if (building.data.buildingType == BuildingType.Tower)
                {
                    var textMesh = _buildingObjects[building.id].GetComponentInChildren<TMPro.TextMeshPro>();
                    if (textMesh != null)
                    {
                        textMesh.text = $"{building.currentAmmo}/{building.data.maxAmmo}";
                        textMesh.color = (building.currentAmmo <= 0 || building.isReloading) ? Color.red : Color.black;
                    }
                }
            }
        }
    }
    
    private BuildingModel _selectedBuilding = null;
    private GameObject _rangeOverlay = null;
    
    void Update()
    {
        if (BuildManager.Instance == null) return;
        
        bool isPlacing = BuildManager.Instance.IsPlacing;
        
        if (!isPlacing && Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
            {
                _selectedBuilding = null;
            }
            else
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int clickX = Mathf.RoundToInt(mousePos.x);
                int clickY = Mathf.RoundToInt(mousePos.y);
                
                var clickedBuilding = GridManager.Instance.GetBuildingAt(clickX, clickY);
                if (clickedBuilding != null) _selectedBuilding = clickedBuilding;
                else _selectedBuilding = null;
            }
        }
        else if (isPlacing)
        {
            _selectedBuilding = null;
        }
        
        // Reset grid tiles to base color
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            var tile = tilesParent.GetChild(i);
            var sr = tile.GetComponent<SpriteRenderer>();
            sr.color = new Color(0.8f, 0.8f, 0.8f);
        }
        
        // Handle Range Overlay
        if (_rangeOverlay == null)
        {
            _rangeOverlay = new GameObject("RangeOverlay");
            var sr = _rangeOverlay.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBoxSprite();
            sr.color = new Color(0f, 1f, 0f, 0.3f);
            sr.sortingOrder = 5;
        }
        
        if (isPlacing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int posX = Mathf.RoundToInt(mousePos.x);
            int posY = Mathf.RoundToInt(mousePos.y);
            
            _rangeOverlay.SetActive(true);
            _rangeOverlay.transform.position = new Vector3(posX, posY, -0.1f);
            
            float size = 1f;
            var data = BuildManager.Instance.BuildingToPlace;
            if (data != null)
            {
                if (data.buildingType == BuildingType.Tower) size = data.attackRange;
                else if (data.buildingType == BuildingType.Factory) size = 1f + 2f * data.connectionRange;
                else if (data.buildingType == BuildingType.Road || data.buildingType == BuildingType.FactorySpeedBuff || data.buildingType == BuildingType.TowerAttackBuff) size = 1f + 2f * data.buffRange;
            }
            
            _rangeOverlay.transform.localScale = new Vector3(size, size, 1f);
            
            var sr = _rangeOverlay.GetComponent<SpriteRenderer>();
            if (BuildManager.Instance.IsValidPlacement(posX, posY)) sr.color = new Color(0f, 1f, 0f, 0.3f);
            else sr.color = new Color(1f, 0f, 0f, 0.3f);
        }
        else if (_selectedBuilding != null)
        {
            _rangeOverlay.SetActive(true);
            _rangeOverlay.transform.position = new Vector3(_selectedBuilding.x, _selectedBuilding.y, -0.1f);
            
            float size = 1f;
            var data = _selectedBuilding.data;
            if (data.buildingType == BuildingType.Tower) size = data.attackRange;
            else if (data.buildingType == BuildingType.Factory) size = 1f + 2f * data.connectionRange;
            else if (data.buildingType == BuildingType.Road || data.buildingType == BuildingType.FactorySpeedBuff || data.buildingType == BuildingType.TowerAttackBuff) size = 1f + 2f * data.buffRange;
            
            _rangeOverlay.transform.localScale = new Vector3(size, size, 1f);
            _rangeOverlay.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.3f);
        }
        else
        {
            _rangeOverlay.SetActive(false);
        }
        
        foreach (var building in GridManager.Instance.GetAllBuildings())
        {
            if (_buildingObjects.ContainsKey(building.id))
            {
                if (building.data.buildingType == BuildingType.Tower)
                {
                    var textMesh = _buildingObjects[building.id].GetComponentInChildren<TMPro.TextMeshPro>();
                    if (textMesh != null)
                    {
                        textMesh.text = $"{building.currentAmmo}/{building.data.maxAmmo}";
                        textMesh.color = (building.currentAmmo <= 0 || building.isReloading) ? Color.red : Color.black;
                    }
                }
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
        
        if (model.data.buildingType == BuildingType.Tower)
        {
            var textGo = new GameObject("AmmoText");
            textGo.transform.SetParent(buildingGo.transform);
            textGo.transform.localPosition = new Vector3(0, 0.4f, -0.1f);
            
            var textMesh = textGo.AddComponent<TMPro.TextMeshPro>();
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;
            textMesh.fontSize = 2.5f;
            textMesh.text = $"{model.currentAmmo}/{model.data.maxAmmo}";
            textMesh.color = Color.black;
            textMesh.rectTransform.sizeDelta = new Vector2(1, 1);
        }

        if (model.data.buildingType == BuildingType.Road)
        {
            buildingGo.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            sr.color = Color.gray;
        }
            
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
