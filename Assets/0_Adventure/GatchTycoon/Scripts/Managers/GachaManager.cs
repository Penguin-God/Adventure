using UnityEngine;
using GatchTycoon.Data;
using GatchTycoon.Domain;
using System.Linq;

namespace GatchTycoon.Managers
{
    public class GachaManager : MonoBehaviour
    {
        public static GachaManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public void DrawGacha()
        {
            var config = GridManager.Instance.gameConfig;
            if (config == null || config.gachaPool == null || config.gachaPool.Count == 0) return;
            
            if (CurrencyManager.Instance.SpendCurrency(config.gachaCurrency, config.gachaCost))
            {
                var randomBuilding = config.gachaPool[Random.Range(0, config.gachaPool.Count)];
                
                var size = GridManager.Instance.GetGridSize();
                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        if (GridManager.Instance.IsEmpty(x, y))
                        {
                            GridManager.Instance.PlaceBuilding(randomBuilding, x, y);
                            return;
                        }
                    }
                }
                
                CurrencyManager.Instance.AddCurrency(config.gachaCurrency, config.gachaCost);
                Debug.Log("Grid is full!");
            }
        }
    }
}
