using UnityEngine;
using GatchTycoon.Managers;
using GatchTycoon.Data;

namespace GatchTycoon.Managers
{
    public class GameInitializer : MonoBehaviour
    {
        void Start()
        {
            var cityHallData = Resources.Load<BuildingDataSO>("CityHallData");
            if (cityHallData != null)
            {
                GridManager.Instance.PlaceBuilding(cityHallData, 1, 1);
            }
            else
            {
                Debug.LogWarning("CityHallData not found in Resources!");
            }
            
            // Set starting gold according to design doc
            CurrencyManager.Instance.AddCurrency(Domain.CurrencyType.Gold, 5000);
        }
    }
}
