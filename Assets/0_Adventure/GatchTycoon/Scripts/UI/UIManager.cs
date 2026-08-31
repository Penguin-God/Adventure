using UnityEngine;
using TMPro;
using GatchTycoon.Managers;
using GatchTycoon.Domain;
using System.Linq;

namespace GatchTycoon.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        public TextMeshProUGUI topBarText;
        public UnityEngine.UI.Button gachaButton;
        public UnityEngine.UI.Button upgradeCityHallButton;
        public UnityEngine.UI.Button combineUIButton;
        
        public GameObject combinePopup;
        public BuildingInfoPopupUI infoPopup;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            if (gachaButton != null)
                gachaButton.onClick.AddListener(OnGachaClicked);
                
            if (upgradeCityHallButton != null)
                upgradeCityHallButton.onClick.AddListener(OnUpgradeCityHallClicked);
                
            if (combineUIButton != null)
                combineUIButton.onClick.AddListener(() => combinePopup.SetActive(true));
        }
        
        void Update()
        {
            if (GridManager.Instance == null || topBarText == null) return;
            
            int gold = CurrencyManager.Instance != null ? CurrencyManager.Instance.GetCurrency(CurrencyType.Gold) : 0;
            var buildings = GridManager.Instance.GetAllBuildings();
            var assignments = GridDomainLogic.CalculateWorkerAssignments(buildings);
            
            int maxResidents = 0;
            int currentResidents = 0;
            
            foreach (var r in buildings.Where(b => b.data.category == BuildingCategory.Residence))
            {
                maxResidents += r.data.capacity;
                currentResidents += (int)(r.data.capacity * GridDomainLogic.CalculateOccupancyRate(r, buildings));
            }
            
            int maxJobs = buildings.Where(b => b.data.category == BuildingCategory.Work).Sum(b => b.data.totalJobs);
            int currentWorkers = assignments.Values.Sum();
            
            topBarText.text = $"Gold: {gold} | Residents: {currentResidents}/{maxResidents} | Workers: {currentWorkers}/{maxJobs}";
            
            if (upgradeCityHallButton != null)
            {
                var nextLevel = GridManager.Instance.gameConfig.cityHallLevels.FirstOrDefault(l => l.level == GridManager.Instance.currentCityHallLevel + 1);
                var upgText = upgradeCityHallButton.GetComponentInChildren<TextMeshProUGUI>();
                if (upgText != null)
                {
                    if (nextLevel != null) upgText.text = $"Upgrade City Hall\n({nextLevel.upgradeCost}G)";
                    else upgText.text = "City Hall MAX";
                }
            }
        }
        
        private void OnGachaClicked()
        {
            GachaManager.Instance.DrawGacha();
        }
        
        private void OnUpgradeCityHallClicked()
        {
            GridManager.Instance.UpgradeCityHall();
        }
        
        public void ShowBuildingInfo(string modelId)
        {
            if (infoPopup == null) return;
            var buildings = GridManager.Instance.GetAllBuildings();
            var model = buildings.FirstOrDefault(b => b.id == modelId);
            if (model != null)
            {
                infoPopup.ShowInfo(model);
                GridRenderer.Instance.HighlightRange(model);
            }
        }
    }
}
