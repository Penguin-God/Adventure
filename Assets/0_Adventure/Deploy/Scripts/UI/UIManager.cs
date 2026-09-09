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
    private Button _sellButton;
    private TextMeshProUGUI _bottomInfoText;
    
    void Update()
    {
        if (DefenseManager.Instance != null && BuildManager.Instance != null && MonsterManager.Instance != null)
        {
            if (!_buttonsCreated && BuildManager.Instance.availableBuildings != null && BuildManager.Instance.availableBuildings.Count > 0)
            {
                CreateButtons();
                CreateDynamicUI();
                _buttonsCreated = true;
            }
            
            int timeInSeconds = Mathf.FloorToInt(DefenseManager.Instance.elapsedTime);
            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;
            string timeString = $"{minutes:00}:{seconds:00}";
            
            topBarText.text = $"Gold: {DefenseManager.Instance.currentGold} | Monsters: {MonsterManager.Instance.GetActiveMonsters().Count()} | Time: {timeString}";
            
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(BuildManager.Instance.IsPlacing);
            }
            
            UpdateSelectionUI();
        }
    }
    
    private void CreateDynamicUI()
    {
        var canvas = transform.parent;
        
        // Sell Button
        var sellBtnGo = new GameObject("SellButton");
        sellBtnGo.transform.SetParent(canvas, false);
        sellBtnGo.AddComponent<Image>().color = new Color(1f, 0.8f, 0.2f);
        _sellButton = sellBtnGo.AddComponent<Button>();
        var sellRt = sellBtnGo.GetComponent<RectTransform>();
        sellRt.anchorMin = new Vector2(1, 0.5f);
        sellRt.anchorMax = new Vector2(1, 0.5f);
        sellRt.pivot = new Vector2(1f, 0.5f);
        sellRt.anchoredPosition = new Vector2(-20, 0);
        sellRt.sizeDelta = new Vector2(120, 60);
        var sellTextGo = new GameObject("Text");
        sellTextGo.transform.SetParent(sellBtnGo.transform, false);
        var sellText = sellTextGo.AddComponent<TextMeshProUGUI>();
        sellText.text = "Sell";
        sellText.color = Color.black;
        sellText.alignment = TextAlignmentOptions.Center;
        sellText.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 60);
        
        _sellButton.onClick.AddListener(OnSellClicked);
        _sellButton.gameObject.SetActive(false);
        
        // Bottom Info Text (Input info for factories)
        var infoGo = new GameObject("BottomInfoText");
        infoGo.transform.SetParent(canvas, false);
        _bottomInfoText = infoGo.AddComponent<TextMeshProUGUI>();
        _bottomInfoText.alignment = TextAlignmentOptions.Center;
        _bottomInfoText.fontSize = 24;
        _bottomInfoText.color = Color.black;
        var infoRt = infoGo.GetComponent<RectTransform>();
        infoRt.anchorMin = new Vector2(0.5f, 0);
        infoRt.anchorMax = new Vector2(0.5f, 0);
        infoRt.pivot = new Vector2(0.5f, 0);
        infoRt.anchoredPosition = new Vector2(0, 120);
        infoRt.sizeDelta = new Vector2(600, 40);
    }
    
    private void UpdateSelectionUI()
    {
        if (_sellButton == null || _bottomInfoText == null) return;
        
        var gridRenderer = Object.FindObjectOfType<GridRenderer>();
        if (gridRenderer != null && gridRenderer.SelectedBuilding != null)
        {
            var selected = gridRenderer.SelectedBuilding;
            
            // Cannot sell fixed buildings
            if (selected.data.buildingType == BuildingType.Mine || selected.data.buildingType == BuildingType.Entrance)
                _sellButton.gameObject.SetActive(false);
            else
                _sellButton.gameObject.SetActive(true);
                
            // Update input info if Factory
            if (selected.data.buildingType == BuildingType.Factory)
            {
                string info = "";
                if (selected.data.inputType1 != ResourceType.None)
                {
                    info += $"{selected.data.inputType1}: {selected.currentInput1}/{selected.data.maxInputCapacity}   ";
                }
                if (selected.data.inputType2 != ResourceType.None)
                {
                    info += $"{selected.data.inputType2}: {selected.currentInput2}/{selected.data.maxInputCapacity}";
                }
                _bottomInfoText.text = info;
            }
            else
            {
                _bottomInfoText.text = "";
            }
        }
        else
        {
            _sellButton.gameObject.SetActive(false);
            _bottomInfoText.text = "";
        }
    }
    
    private void OnSellClicked()
    {
        var gridRenderer = Object.FindObjectOfType<GridRenderer>();
        if (gridRenderer != null && gridRenderer.SelectedBuilding != null)
        {
            var selected = gridRenderer.SelectedBuilding;
            if (selected.data.buildingType == BuildingType.Mine || selected.data.buildingType == BuildingType.Entrance) return;
            
            int refund = Mathf.FloorToInt(selected.data.cost * 0.9f);
            DefenseManager.Instance.AddGold(refund);
            GridManager.Instance.RemoveBuilding(selected.id);
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
