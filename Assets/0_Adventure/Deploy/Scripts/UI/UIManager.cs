using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI topBarText;
    public Transform buttonsContainer;
    public Button cancelButton;
    
    void Start()
    {
        if (cancelButton != null) cancelButton.onClick.AddListener(() => BuildManager.Instance.CancelPlacement());
        
        // Dynamic buttons will be created in Update when BuildManager is ready
    }
    
    private bool _buttonsCreated = false;
    
    void Update()
    {
        if (DefenseManager.Instance != null && BuildManager.Instance != null && MonsterManager.Instance != null)
        {
            if (!_buttonsCreated && BuildManager.Instance.availableBuildings != null && BuildManager.Instance.availableBuildings.Count > 0)
            {
                CreateButtons();
                _buttonsCreated = true;
            }
            
            topBarText.text = $"Defense Gold: {DefenseManager.Instance.currentGold} | Monsters: {MonsterManager.Instance.GetActiveMonsters().Count()}";
            
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(BuildManager.Instance.IsPlacing);
            }
        }
    }
    
    private void CreateButtons()
    {
        foreach (var data in BuildManager.Instance.availableBuildings)
        {
            var btnGo = new GameObject($"Btn_{data.buildingName}");
            btnGo.transform.SetParent(buttonsContainer, false);
            btnGo.AddComponent<Image>().color = Color.white;
            var btn = btnGo.AddComponent<Button>();
            
            var rt = btnGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150, 80);
            
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            var textMesh = textGo.AddComponent<TextMeshProUGUI>();
            textMesh.text = $"{data.buildingName}\n({data.cost}G)";
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 80);
            
            var capturedData = data; // capture for lambda
            btn.onClick.AddListener(() => BuildManager.Instance.StartPlacement(capturedData));
        }
    }
}
