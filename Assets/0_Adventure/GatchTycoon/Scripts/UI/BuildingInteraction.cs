using UnityEngine;
using System.Linq;
using GatchTycoon.Managers;

namespace GatchTycoon.UI
{
    public class BuildingInteraction : MonoBehaviour
    {
        public string modelId;
        
        private Vector3 _offset;
        private Vector3 _startPosition;
        private Vector3 _mouseDownPos;
        private Camera _mainCamera;
        private bool _isDragging;
        
        void Start()
        {
            _mainCamera = Camera.main;
            
            var col = GetComponent<Collider>();
            if (col == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
        
        void OnMouseDown()
        {
            if (GridManager.Instance == null || GridRenderer.Instance == null) return;
            var buildings = GridManager.Instance.GetAllBuildings();
            var model = buildings.FirstOrDefault(b => b.id == modelId);
            if (model == null) return; 
            
            _startPosition = transform.position;
            _mouseDownPos = Input.mousePosition;
            _offset = transform.position - GetMouseWorldPos();
            _isDragging = true;
        }
        
        void OnMouseDrag()
        {
            if (!_isDragging) return;
            if (Vector3.Distance(Input.mousePosition, _mouseDownPos) > 10f)
            {
                Vector3 pos = GetMouseWorldPos() + _offset;
                pos.y = 0.5f; 
                transform.position = pos;
            }
        }
        
        void OnMouseUp()
        {
            if (!_isDragging) return;
            _isDragging = false;
            
            // Check if it was just a click
            if (Vector3.Distance(Input.mousePosition, _mouseDownPos) <= 10f)
            {
                UIManager.Instance.ShowBuildingInfo(modelId);
                return;
            }
            
            var buildings = GridManager.Instance.GetAllBuildings();
            var model = buildings.FirstOrDefault(b => b.id == modelId);
            if (model == null) return;
            
            var targetGridPos = GridRenderer.Instance.WorldToGridPosition(transform.position);
            
            if (targetGridPos.x == model.x && targetGridPos.y == model.y)
            {
                transform.position = _startPosition;
                return;
            }
            
            bool success = GridManager.Instance.SwapOrMoveBuilding(model.x, model.y, targetGridPos.x, targetGridPos.y);
            
            if (!success)
            {
                transform.position = _startPosition;
            }
        }
        
        private Vector3 GetMouseWorldPos()
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            return transform.position;
        }
    }
}
