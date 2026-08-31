using UnityEngine;
using System.Collections.Generic;
using GatchTycoon.Domain;
using GatchTycoon.Managers;

namespace GatchTycoon.UI
{
    public class GridRenderer : MonoBehaviour
    {
        public static GridRenderer Instance { get; private set; }
        
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
                    if (!_tiles.ContainsKey(pos))
                    {
                        var go = new GameObject($"Tile_{x}_{y}");
                        go.transform.position = new Vector3(x * tileSize, -0.01f, y * tileSize);
                        go.transform.SetParent(tilesParent);
                        go.transform.rotation = Quaternion.Euler(90, 0, 0);
                        
                        var sr = go.AddComponent<SpriteRenderer>();
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, Color.white);
                        tex.Apply();
                        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                        go.transform.localScale = new Vector3(tileSize * 0.95f, tileSize * 0.95f, 1);
                        sr.color = new Color(0.8f, 0.8f, 0.8f);
                        
                        _tiles[pos] = go;
                    }
                }
            }
            
            var buildings = GridManager.Instance.GetAllBuildings();
            var assignments = GridDomainLogic.CalculateWorkerAssignments(buildings);
            
            foreach (var b in buildings)
            {
                if (_buildingObjects.ContainsKey(b.id))
                {
                    _buildingObjects[b.id].transform.position = new Vector3(b.x * tileSize, 0, b.y * tileSize);
                    UpdateBuildingColor(b, assignments);
                }
                else
                {
                    OnBuildingPlaced(b);
                }
            }
        }
        
        private void UpdateBuildingColor(BuildingModel model, Dictionary<string, int> assignments)
        {
            if (!_buildingObjects.ContainsKey(model.id)) return;
            var go = _buildingObjects[model.id];
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;
            
            Color catColor = Color.white;
            switch (model.data.category)
            {
                case BuildingCategory.CityHall: catColor = Color.yellow; break;
                case BuildingCategory.Residence: catColor = Color.green; break;
                case BuildingCategory.Work: 
                    catColor = Color.blue; 
                    if (assignments != null && assignments.ContainsKey(model.id) && assignments[model.id] > 0)
                    {
                        catColor = Color.cyan; // Active!
                    }
                    break;
                case BuildingCategory.Convenience: catColor = Color.magenta; break;
                case BuildingCategory.Public: catColor = new Color(1f, 0.5f, 0f); break; // Orange
                case BuildingCategory.Special: catColor = Color.gray; break;
            }
            
            float darkenFactor = Mathf.Max(0.2f, 1.0f - ((model.data.level - 1) * 0.3f));
            catColor = new Color(catColor.r * darkenFactor, catColor.g * darkenFactor, catColor.b * darkenFactor, 1f);
            
            if (renderer is SpriteRenderer sr)
            {
                sr.color = catColor;
            }
            else
            {
                renderer.material.color = catColor;
            }
        }
        
        private void OnBuildingPlaced(BuildingModel model)
        {
            if (_buildingObjects.ContainsKey(model.id)) return;
            
            GameObject go = new GameObject($"Building_{model.data.buildingName}_{model.id}");
            go.transform.position = new Vector3(model.x * tileSize, 0, model.y * tileSize);
            go.transform.SetParent(buildingsParent);
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            
            var sr = go.AddComponent<SpriteRenderer>();
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            
            go.transform.localScale = new Vector3(tileSize * 0.8f, tileSize * 0.8f, 1);
            
            var interaction = go.AddComponent<BuildingInteraction>();
            interaction.modelId = model.id;
            
            _buildingObjects[model.id] = go;
            UpdateBuildingColor(model, null);
        }
        
        private void OnBuildingRemoved(BuildingModel model)
        {
            if (_buildingObjects.ContainsKey(model.id))
            {
                Destroy(_buildingObjects[model.id]);
                _buildingObjects.Remove(model.id);
            }
        }
        
        public void HighlightRange(BuildingModel model)
        {
            foreach (var kvp in _tiles)
            {
                var sr = kvp.Value.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = new Color(0.8f, 0.8f, 0.8f);
            }
            
            if (model == null) return;
            
            RangePattern pattern = RangePattern.None;
            if (model.data.category == BuildingCategory.Residence) pattern = model.data.commutePattern;
            else if (model.data.category == BuildingCategory.Convenience || model.data.category == BuildingCategory.Public) pattern = model.data.effectPattern;
            
            if (pattern != RangePattern.None)
            {
                foreach (var kvp in _tiles)
                {
                    if (GridDomainLogic.IsInPattern(model.x, model.y, kvp.Key.x, kvp.Key.y, pattern))
                    {
                        var sr = kvp.Value.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(1f, 0.5f, 0.5f); // Reddish highlight
                    }
                }
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
