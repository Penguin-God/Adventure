using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using GatchTycoon.Managers;
using GatchTycoon.Data;
using System.Collections.Generic;

namespace GatchTycoon.UI
{
    public class CombinePopupUI : MonoBehaviour
    {
        public Transform contentParent;
        public GameObject recipeItemPrefab;
        public Button closeButton;
        
        private List<GameObject> _items = new List<GameObject>();
        
        void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
        
        void OnEnable()
        {
            RefreshUI();
        }
        
        public void RefreshUI()
        {
            foreach (var item in _items) Destroy(item);
            _items.Clear();
            
            if (GridManager.Instance == null || GridManager.Instance.gameConfig == null) return;
            
            // Gather all combinable building types
            var allTypes = Resources.LoadAll<BuildingDataSO>("");
            var combinableTypes = allTypes.Where(t => t.nextLevelBuilding != null).ToList();
            
            foreach (var type in combinableTypes)
            {
                var go = Instantiate(recipeItemPrefab, contentParent);
                _items.Add(go);
                
                var titleText = go.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
                titleText.text = $"{type.buildingName} x{type.requiredCount} -> {type.nextLevelBuilding.buildingName}";
                
                var btn = go.transform.Find("CombineBtn").GetComponent<Button>();
                bool canCombine = GridManager.Instance.CanCombine(type);
                btn.interactable = canCombine;
                
                btn.onClick.AddListener(() => {
                    if (GridManager.Instance.ExecuteCombine(type))
                    {
                        RefreshUI(); // Refresh list after combine
                    }
                });
            }
        }
    }
}
