using UnityEngine;
using GatchTycoon.Domain;
using System.Linq;

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
            if (!buildings.Any()) return;
            
            var assignments = GridDomainLogic.CalculateWorkerAssignments(buildings);
            
            int totalGold = 0;
            foreach (var b in buildings)
            {
                totalGold += GridDomainLogic.CalculateBuildingGold(b, buildings, assignments);
            }
            
            if (totalGold > 0)
            {
                CurrencyManager.Instance.AddCurrency(CurrencyType.Gold, totalGold);
            }
        }
    }
}
