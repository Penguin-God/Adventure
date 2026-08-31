using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GatchTycoon.Domain;

namespace GatchTycoon.UI
{
    public class BuildingInfoPopupUI : MonoBehaviour
    {
        public TextMeshProUGUI infoText;
        public Button closeButton;
        
        void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(() => {
                    gameObject.SetActive(false);
                    if (GridRenderer.Instance != null)
                    {
                        GridRenderer.Instance.HighlightRange(null);
                    }
                });
        }
        
        void OnEnable()
        {
            var rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.05f, 0.2f);
                rt.anchorMax = new Vector2(0.35f, 0.8f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }
        
        public void ShowInfo(BuildingModel model)
        {
            gameObject.SetActive(true);
            
            string info = $"<size=30><b>{model.data.buildingName}</b></size>\n";
            info += $"Level: {model.data.level} | Type: {model.data.category}\n\n";
            info += $"Base Gold: {model.data.baseGoldPerHour} / 10s\n";
            
            if (model.data.category == BuildingCategory.Residence)
            {
                info += $"Capacity: {model.data.capacity}\n";
                info += $"Base Occupancy: {model.data.baseOccupancyRate * 100}%\n";
                info += $"Commute Range: {GetPatternName(model.data.commutePattern)}\n";
            }
            else if (model.data.category == BuildingCategory.Work)
            {
                info += $"Total Jobs: {model.data.totalJobs}\n";
                info += $"Profit per Worker: {model.data.profitPerWorker}\n";
            }
            else if (model.data.category == BuildingCategory.Convenience)
            {
                info += $"Buff: +{model.data.buffAmount * 100}% Money Efficiency\n";
                info += $"Effect Range: {GetPatternName(model.data.effectPattern)}\n";
            }
            else if (model.data.category == BuildingCategory.Public)
            {
                info += $"Buff: +{model.data.buffAmount * 100}% Occupancy Rate\n";
                info += $"Effect Range: {GetPatternName(model.data.effectPattern)}\n";
            }
            
            infoText.text = info;
        }
        
        private string GetPatternName(RangePattern pattern)
        {
            switch(pattern)
            {
                case RangePattern.LeftRight: return "Left & Right 1";
                case RangePattern.Cross: return "Cross (Up/Down/Left/Right 1)";
                case RangePattern.Square3x3: return "3x3 Area";
                case RangePattern.TopDiagonals: return "Top Diagonals";
                case RangePattern.AllDiagonals: return "All 4 Diagonals";
                default: return "None";
            }
        }
    }
}
