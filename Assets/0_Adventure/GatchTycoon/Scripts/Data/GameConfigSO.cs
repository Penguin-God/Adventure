using UnityEngine;
using System.Collections.Generic;
using GatchTycoon.Domain;

namespace GatchTycoon.Data
{
    [System.Serializable]
    public class CityHallLevelInfo
    {
        public int level;
        public int gridSizeX;
        public int gridSizeY;
        public int upgradeCost;
        public CurrencyType costCurrency;
    }

    [CreateAssetMenu(fileName = "NewGameConfig", menuName = "GatchTycoon/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        public List<CityHallLevelInfo> cityHallLevels;
        public int gachaCost;
        public CurrencyType gachaCurrency;
        
        public List<BuildingDataSO> gachaPool;
    }
}
