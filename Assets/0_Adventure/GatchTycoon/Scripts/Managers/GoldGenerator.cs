using UnityEngine;
using GatchTycoon.Domain;

namespace GatchTycoon.Managers
{
    public class GoldGenerator : MonoBehaviour
    {
        public float tickRate = 1.0f; 
        private float _timer;
        
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= tickRate)
            {
                _timer -= tickRate;
                GenerateGold();
            }
        }
        
        private void GenerateGold()
        {
            var buildings = GridManager.Instance.GetAllBuildings();
            int totalGold = 0;
            
            foreach (var b in buildings)
            {
                if (b.data.category == BuildingCategory.CityHall)
                {
                    totalGold += b.data.goldPerHour;
                }
                else if (b.data.category == BuildingCategory.Work)
                {
                    totalGold += GridDomainLogic.CalculateCorporationGold(b, buildings);
                }
            }
            
            if (totalGold > 0)
            {
                CurrencyManager.Instance.AddCurrency(CurrencyType.Gold, totalGold);
            }
        }
    }
}
