using UnityEngine;
using TMPro;
using GatchTycoon.Managers;
using GatchTycoon.Domain;

namespace GatchTycoon.UI
{
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI goldText;
        public UnityEngine.UI.Button gachaButton;
        public UnityEngine.UI.Button upgradeCityHallButton;
        
        void Start()
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
                UpdateCurrencyUI(CurrencyType.Gold, CurrencyManager.Instance.GetCurrency(CurrencyType.Gold));
            }
            
            if (gachaButton != null)
                gachaButton.onClick.AddListener(OnGachaClicked);
                
            if (upgradeCityHallButton != null)
                upgradeCityHallButton.onClick.AddListener(OnUpgradeCityHallClicked);
        }
        
        void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyUI;
            }
        }
        
        private void UpdateCurrencyUI(CurrencyType type, int amount)
        {
            if (type == CurrencyType.Gold && goldText != null)
            {
                goldText.text = $"Gold: {amount}";
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
    }
}
