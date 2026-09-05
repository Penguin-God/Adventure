using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI topBarText;
    public Button summonButton;
    public Button buildRoadButton;
    
    void Start()
    {
        summonButton.onClick.AddListener(() => DeckManager.Instance.Summon());
        buildRoadButton.onClick.AddListener(() => DeckManager.Instance.BuildRoad());
    }
    
    void Update()
    {
        if (DefenseManager.Instance != null && DeckManager.Instance != null && MonsterManager.Instance != null)
        {
            topBarText.text = $"Defense Gold: {DefenseManager.Instance.currentGold} | Monsters: {MonsterManager.Instance.GetActiveMonsters().Count()}";
            
            var nextData = DeckManager.Instance.PeekNext();
            if (nextData != null)
            {
                summonButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Summon {nextData.buildingName}\n({DeckManager.Instance.summonCost}G)";
            }
            else
            {
                summonButton.GetComponentInChildren<TextMeshProUGUI>().text = "No Cards Left";
            }
        }
    }
}
