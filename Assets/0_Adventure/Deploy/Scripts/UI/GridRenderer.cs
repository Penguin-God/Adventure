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
        GridManager.Instance.OnBuildingRemoved += OnBuildingRemoved;
        DrawGrid();
    }
    
    private void OnBuildingRemoved(string id)
    {
        if (_buildingObjects.ContainsKey(id))
        {
            Destroy(_buildingObjects[id]);
            _buildingObjects.Remove(id);
        }
        if (SelectedBuilding != null && SelectedBuilding.id == id) SelectedBuilding = null;
    }
    
    private void DrawGrid()
    {
        if (tilesParent.childCount == 0)
        {
            // Village
            for (int posX = 0; posX < 15; posX++)
            {
                for (int posY = 0; posY < 8; posY++)
                {
                    CreateGridTile(posX, posY, new Color(0.8f, 0.8f, 0.8f));
                }
            }
            // Tower Zones (4 width x 2 height)
            for (int posY = -3; posY <= -2; posY++)
            {
                for (int posX = 0; posX <= 3; posX++) CreateGridTile(posX, posY, new Color(0.7f, 0.7f, 0.85f));
                for (int posX = 5; posX <= 8; posX++) CreateGridTile(posX, posY, new Color(0.7f, 0.7f, 0.85f));
                for (int posX = 11; posX <= 14; posX++) CreateGridTile(posX, posY, new Color(0.7f, 0.7f, 0.85f));
            }
            
            // Separator Roads
            CreateGridTile(0, -1, new Color(0.85f, 0.8f, 0.7f));
            CreateGridTile(7, -1, new Color(0.85f, 0.8f, 0.7f));
            CreateGridTile(14, -1, new Color(0.85f, 0.8f, 0.7f));
        }
        
        foreach (var building in GridManager.Instance.GetAllBuildings())
        {
            if (_buildingObjects.ContainsKey(building.id))
            {
                _buildingObjects[building.id].transform.position = new Vector3(building.x, building.y, 0);
                
                var textMesh = _buildingObjects[building.id].GetComponentInChildren<TMPro.TextMeshPro>();
                if (textMesh != null)
                {
                    if (building.data.buildingType == BuildingType.Tower)
                    {
                        textMesh.text = $"{building.currentInput1}/{building.data.maxAmmo}";
                        textMesh.color = building.isShutdown ? Color.red : Color.black;
                    }
                    else if (building.data.buildingType == BuildingType.Factory || building.data.buildingType == BuildingType.Mine)
                    {
                        textMesh.text = $"{building.currentOutput}/{building.data.maxOutputCapacity}";
                        textMesh.color = building.isShutdown ? Color.red : Color.black;
                    }
                    else
                    {
                        textMesh.text = "";
                    }
                }
            }
        }
    }
    
    public BuildingModel SelectedBuilding { get; private set; }
    private GameObject _rangeOverlay = null;
    
    private BuildingModel _draggingBuilding = null;
    
    void Update()
    {
        if (BuildManager.Instance == null) return;
        
        bool isPlacing = BuildManager.Instance.IsPlacing;
        
        if (!isPlacing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int gridX = Mathf.RoundToInt(mousePos.x);
            int gridY = Mathf.RoundToInt(mousePos.y);
            
            if (Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current != null && 
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
                {
                    SelectedBuilding = null;
                    _draggingBuilding = null;
                }
                else
                {
                    var clickedBuilding = GridManager.Instance.GetBuildingAt(gridX, gridY);
                    SelectedBuilding = clickedBuilding;
                    
                    if (clickedBuilding != null && clickedBuilding.data.buildingType != BuildingType.Mine && clickedBuilding.data.buildingType != BuildingType.Entrance) 
                    {
                        _draggingBuilding = clickedBuilding;
                    }
                    else
                    {
                        _draggingBuilding = null;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && _draggingBuilding != null)
            {
                GridManager.Instance.MoveBuilding(_draggingBuilding.id, gridX, gridY);
                _draggingBuilding = null;
            }
        }
        else if (isPlacing)
        {
            SelectedBuilding = null;
            _draggingBuilding = null;
        }
        
        // Reset grid tiles to base color
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            var tile = tilesParent.GetChild(i);
            var sr = tile.GetComponent<SpriteRenderer>();
            var baseColor = tile.GetComponent<TileBaseColor>();
            if (baseColor != null) sr.color = baseColor.baseColor;
            else sr.color = new Color(0.8f, 0.8f, 0.8f);
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
        else if (_draggingBuilding != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int gridX = Mathf.RoundToInt(mousePos.x);
            int gridY = Mathf.RoundToInt(mousePos.y);
            
            _rangeOverlay.SetActive(true);
            _rangeOverlay.transform.position = new Vector3(gridX, gridY, -0.1f);
            
            float size = 1f;
            var data = _draggingBuilding.data;
            if (data.buildingType == BuildingType.Tower) size = data.attackRange;
            else if (data.buildingType == BuildingType.Factory) size = 1f + 2f * data.connectionRange;
            else if (data.buildingType == BuildingType.Road || data.buildingType == BuildingType.FactorySpeedBuff || data.buildingType == BuildingType.TowerAttackBuff) size = 1f + 2f * data.buffRange;
            
            _rangeOverlay.transform.localScale = new Vector3(size, size, 1f);
            
            bool valid = GridManager.Instance.GetBuildingAt(gridX, gridY) == null && GridManager.Instance.IsValidCoordinateForType(data.buildingType, gridX, gridY);
            _rangeOverlay.GetComponent<SpriteRenderer>().color = valid ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
        }
        else if (SelectedBuilding != null)
        {
            _rangeOverlay.SetActive(true);
            _rangeOverlay.transform.position = new Vector3(SelectedBuilding.x, SelectedBuilding.y, -0.1f);
            
            float size = 1f;
            var data = SelectedBuilding.data;
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
                if (_draggingBuilding == building) 
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    int gridX = Mathf.RoundToInt(mousePos.x);
                    int gridY = Mathf.RoundToInt(mousePos.y);
                    _buildingObjects[building.id].transform.position = new Vector3(gridX, gridY, 0);
                }
                else
                {
                    _buildingObjects[building.id].transform.position = new Vector3(building.x, building.y, 0);
                }
                
                var textMesh = _buildingObjects[building.id].GetComponentInChildren<TMPro.TextMeshPro>();
                if (textMesh != null)
                {
                    if (building.data.buildingType == BuildingType.Tower)
                    {
                        textMesh.text = $"{building.currentInput1}/{building.data.maxAmmo}";
                        textMesh.color = building.isShutdown ? Color.red : Color.black;
                    }
                    else if (building.data.buildingType == BuildingType.Factory || building.data.buildingType == BuildingType.Mine)
                    {
                        textMesh.text = $"{building.currentOutput}/{building.data.maxOutputCapacity}";
                        textMesh.color = building.isShutdown ? Color.red : Color.black;
                        
                        if (building.data.buildingType == BuildingType.Factory)
                        {
                            var inputText = _buildingObjects[building.id].transform.Find("InputText");
                            if (inputText != null)
                            {
                                var tmp = inputText.GetComponent<TMPro.TextMeshPro>();
                                string info = "";
                                if (building.data.inputType1 != ResourceType.None) info += $"{building.data.inputType1}:{building.currentInput1}/{building.data.maxInputCapacity} ";
                                if (building.data.inputType2 != ResourceType.None) info += $"{building.data.inputType2}:{building.currentInput2}/{building.data.maxInputCapacity}";
                                tmp.text = info.Trim();
                            }
                        }
                    }
                    else
                    {
                        textMesh.text = "";
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
        
        if (model.data.buildingType == BuildingType.Tower || model.data.buildingType == BuildingType.Factory || model.data.buildingType == BuildingType.Mine)
        {
            var textGo = new GameObject("AmmoText");
            textGo.transform.SetParent(buildingGo.transform);
            textGo.transform.localPosition = new Vector3(0, 0.4f, -0.1f);
            
            var textMesh = textGo.AddComponent<TMPro.TextMeshPro>();
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;
            textMesh.fontSize = 2.5f;
            textMesh.text = "";
            
            if (model.data.buildingType == BuildingType.Factory)
            {
                var inputTextGo = new GameObject("InputText");
                inputTextGo.transform.SetParent(buildingGo.transform);
                inputTextGo.transform.localPosition = new Vector3(0, -0.4f, -0.1f);
                
                var inputTmp = inputTextGo.AddComponent<TMPro.TextMeshPro>();
                inputTmp.alignment = TMPro.TextAlignmentOptions.Center;
                inputTmp.fontSize = 1.8f;
                inputTmp.color = Color.blue;
                inputTmp.text = "";
            }
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
    
    private void CreateGridTile(int posX, int posY, Color defaultColor)
    {
        var tileGo = new GameObject($"Tile_{posX}_{posY}");
        tileGo.transform.position = new Vector3(posX, posY, 0.1f); 
        tileGo.transform.SetParent(tilesParent);
        var sr = tileGo.AddComponent<SpriteRenderer>();
        sr.sprite = CreateBoxSprite();
        sr.color = defaultColor;
        tileGo.transform.localScale = new Vector3(0.95f, 0.95f, 1);
        
        // Store base color so we can reset correctly
        var baseColorComponent = tileGo.AddComponent<TileBaseColor>();
        baseColorComponent.baseColor = defaultColor;
    }
    
    private Sprite CreateBoxSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}

public class TileBaseColor : MonoBehaviour
{
    public Color baseColor;
}
