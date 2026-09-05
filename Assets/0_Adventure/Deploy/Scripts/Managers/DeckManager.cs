using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    
    public List<BuildingDataSO> fullDeck;
    private Queue<BuildingDataSO> _deckQueue;
    
    public int summonCost = 100;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (fullDeck == null || fullDeck.Count == 0)
        {
            fullDeck = new List<BuildingDataSO>(Resources.LoadAll<BuildingDataSO>("Buildings"));
        }
        
        _deckQueue = new Queue<BuildingDataSO>(fullDeck);
    }
    
    public BuildingDataSO PeekNext()
    {
        if (_deckQueue.Count == 0) return null;
        return _deckQueue.Peek();
    }
    
    public void Summon()
    {
        if (_deckQueue.Count == 0) return;
        
        if (DefenseManager.Instance.SpendGold(summonCost))
        {
            var data = _deckQueue.Dequeue();
            
            bool placed = false;
            int maxAttempts = 100;
            
            while (!placed && maxAttempts > 0)
            {
                int posX, posY;
                if (data.buildingType == BuildingType.Factory || data.buildingType == BuildingType.FactorySpeedBuff)
                {
                    posX = Random.Range(3, 7);
                    posY = Random.Range(3, 7);
                }
                else
                {
                    posX = Random.Range(0, 10);
                    posY = Random.Range(0, 10);
                    if (posX >= 3 && posX <= 6 && posY >= 3 && posY <= 6) continue; 
                }
                
                placed = GridManager.Instance.PlaceBuilding(data, posX, posY);
                maxAttempts--;
            }
            
            _deckQueue.Enqueue(data); 
        }
    }
    
    public void BuildRoad()
    {
        var roadData = Resources.Load<BuildingDataSO>("Buildings/Road");
        if (roadData == null) return;
        
        if (DefenseManager.Instance.SpendGold(50))
        {
            bool placed = false;
            int maxAttempts = 100;
            while (!placed && maxAttempts > 0)
            {
                int posX = Random.Range(0, 10);
                int posY = Random.Range(0, 10);
                placed = GridManager.Instance.PlaceBuilding(roadData, posX, posY);
                maxAttempts--;
            }
        }
    }
}
