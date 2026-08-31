using UnityEngine;
using System.Collections.Generic;
using GatchTycoon.Domain;
using GatchTycoon.Managers;
using TMPro;

namespace GatchTycoon.UI
{
    public class GridRenderer : MonoBehaviour
    {
        public static GridRenderer Instance { get; private set; }
        
        public GameObject tilePrefab;
        public float tileSize = 1.2f;
        public Transform tilesParent;
        public Transform buildingsParent;
        
        private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<string, GameObject> _buildingObjects = new Dictionary<string, GameObject>();
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            GridManager.Instance.OnGridChanged += DrawGrid;
            GridManager.Instance.OnBuildingPlaced += OnBuildingPlaced;
            GridManager.Instance.OnBuildingRemoved += OnBuildingRemoved;
            
            DrawGrid();
        }
        
        void OnDestroy()
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.OnGridChanged -= DrawGrid;
                GridManager.Instance.OnBuildingPlaced -= OnBuildingPlaced;
                GridManager.Instance.OnBuildingRemoved -= OnBuildingRemoved;
            }
        }
        
        private void DrawGrid()
        {
            var size = GridManager.Instance.GetGridSize();
            
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!_tiles.ContainsKey(pos) && tilePrefab != null)
                    {
                        var go = Instantiate(tilePrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity, tilesParent);
                        go.name = $"Tile_{x}_{y}";
                        _tiles[pos] = go;
                    }
                }
            }
            
            foreach (var b in GridManager.Instance.GetAllBuildings())
            {
                if (_buildingObjects.ContainsKey(b.id))
                {
                    _buildingObjects[b.id].transform.position = new Vector3(b.x * tileSize, 0, b.y * tileSize);
                }
            }
        }
        
        private void OnBuildingPlaced(BuildingModel model)
        {
            GameObject prefab = model.data.prefab;
            bool isTempPrefab = false;
            if (prefab == null) 
            {
                prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                isTempPrefab = true;
            }
            
            var go = Instantiate(prefab, new Vector3(model.x * tileSize, 0, model.y * tileSize), Quaternion.identity, buildingsParent);
            go.name = $"Building_{model.data.buildingName}_{model.id}";
            
            var interaction = go.AddComponent<BuildingInteraction>();
            interaction.modelId = model.id;
            
            var canvasGo = new GameObject("UI");
            canvasGo.transform.SetParent(go.transform, false);
            canvasGo.transform.localPosition = new Vector3(0, 1.5f, 0);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 1);
            
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(canvasGo.transform, false);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = $"Lv.{model.data.level}\n{model.data.buildingName}";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 0.5f;
            textGo.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 1);
            
            _buildingObjects[model.id] = go;
            
            if (isTempPrefab) Destroy(prefab);
        }
        
        private void OnBuildingRemoved(BuildingModel model)
        {
            if (_buildingObjects.ContainsKey(model.id))
            {
                Destroy(_buildingObjects[model.id]);
                _buildingObjects.Remove(model.id);
            }
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / tileSize);
            int y = Mathf.RoundToInt(worldPos.z / tileSize);
            return new Vector2Int(x, y);
        }
    }
}
