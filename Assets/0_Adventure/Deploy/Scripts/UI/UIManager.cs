using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI topBarText;
    public Button summonButton1;
    public Button summonButton2;
    public Button buildRoadButton;
    public Button cancelButton;
    
    void Start()
    {
        if (summonButton1 != null) summonButton1.onClick.AddListener(() => DeckManager.Instance.SummonFromHand(0));
        if (summonButton2 != null) summonButton2.onClick.AddListener(() => DeckManager.Instance.SummonFromHand(1));
        if (buildRoadButton != null) buildRoadButton.onClick.AddListener(() => DeckManager.Instance.BuildRoad());
        if (cancelButton != null) cancelButton.onClick.AddListener(() => DeckManager.Instance.CancelPlacement());
    }
    
    void Update()
    {
        if (DefenseManager.Instance != null && DeckManager.Instance != null && MonsterManager.Instance != null)
        {
            topBarText.text = $"Defense Gold: {DefenseManager.Instance.currentGold} | Monsters: {MonsterManager.Instance.GetActiveMonsters().Count()}";
            
            UpdateButtonText(summonButton1, 0);
            UpdateButtonText(summonButton2, 1);
            
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(DeckManager.Instance.IsPlacing);
            }
        }
    }
    
    private void UpdateButtonText(Button btn, int index)
    {
        if (btn == null) return;
        var textMesh = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh == null) return;
        
        var data = DeckManager.Instance.currentHand[index];
        if (data != null)
        {
            textMesh.text = $"Summon {data.buildingName}\n({data.cost}G)";
        }
        else
        {
            textMesh.text = "Empty";
        }
    }
}
