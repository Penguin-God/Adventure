using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI topBarText;
    public Transform buttonsContainer;
    public Button cancelButton;
    
    private bool _buttonsCreated = false;
    private Button _sellButton;
    private Button _buildModeBtn;
    private GameObject _stageContainer;
    private GameObject _endGameContainer;
    
    void Start()
    {
        if (cancelButton != null) cancelButton.onClick.AddListener(() => {
            if (BuildManager.Instance != null) BuildManager.Instance.CancelPlacement();
        });
        
        bool isLobby = SceneManager.GetActiveScene().name == "Lobby";
        CreateDynamicUI(isLobby);
        
        if (!isLobby)
        {
            CreateDefenseEndGameUI();
        }
    }
    
    private void CreateDynamicUI(bool isLobby)
    {
        var canvas = transform.parent;
        
        // Sell Button
        var sellBtnGo = new GameObject("SellButton");
        sellBtnGo.transform.SetParent(canvas, false);
        sellBtnGo.AddComponent<Image>().color = new Color(1f, 0.8f, 0.2f);
        _sellButton = sellBtnGo.AddComponent<Button>();
        var sellRt = sellBtnGo.GetComponent<RectTransform>();
        sellRt.anchorMin = new Vector2(0.5f, 0f);
        sellRt.anchorMax = new Vector2(0.5f, 0f);
        sellRt.pivot = new Vector2(0.5f, 0f);
        sellRt.anchoredPosition = new Vector2(0, 20);
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
        
        if (isLobby)
        {
            // Build Mode Button
            var buildModeBtnGo = new GameObject("BuildModeButton");
            buildModeBtnGo.transform.SetParent(canvas, false);
            buildModeBtnGo.AddComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
            _buildModeBtn = buildModeBtnGo.AddComponent<Button>();
            var bmRt = buildModeBtnGo.GetComponent<RectTransform>();
            bmRt.anchorMin = new Vector2(1, 0);
            bmRt.anchorMax = new Vector2(1, 0);
            bmRt.pivot = new Vector2(1f, 0f);
            bmRt.anchoredPosition = new Vector2(-20, 20);
            bmRt.sizeDelta = new Vector2(150, 80);
            var bmTextGo = new GameObject("Text");
            bmTextGo.transform.SetParent(buildModeBtnGo.transform, false);
            var bmText = bmTextGo.AddComponent<TextMeshProUGUI>();
            bmText.text = "Build Mode";
            bmText.color = Color.white;
            bmText.alignment = TextAlignmentOptions.Center;
            bmText.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 80);
            _buildModeBtn.onClick.AddListener(() => {
                BuildManager.Instance.ToggleBuildMode();
            });
            
            // Stage Buttons
            _stageContainer = new GameObject("Stages");
            _stageContainer.transform.SetParent(canvas, false);
            var scRt = _stageContainer.AddComponent<RectTransform>();
            // Anchor to Bottom Right, above the Build Mode button
            scRt.anchorMin = new Vector2(1, 0);
            scRt.anchorMax = new Vector2(1, 0);
            scRt.pivot = new Vector2(1f, 0f);
            scRt.anchoredPosition = new Vector2(-20, 120);
            scRt.sizeDelta = new Vector2(150, 500); // Enough for 5 vertical buttons
            var layout = _stageContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.LowerCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            for (int i = 1; i <= 4; i++)
            {
                int stageNum = i;
                var sBtnGo = new GameObject($"Stage_{i}");
                sBtnGo.transform.SetParent(_stageContainer.transform, false);
                var img = sBtnGo.AddComponent<Image>();
                img.color = GameState.unlockedStage >= i ? Color.white : Color.gray;
                var sBtn = sBtnGo.AddComponent<Button>();
                sBtn.interactable = GameState.unlockedStage >= i;
                var sRt = sBtnGo.GetComponent<RectTransform>();
                sRt.sizeDelta = new Vector2(120, 50);
                
                var stTextGo = new GameObject("Text");
                stTextGo.transform.SetParent(sBtnGo.transform, false);
                var stText = stTextGo.AddComponent<TextMeshProUGUI>();
                stText.text = $"Stage {i}";
                stText.color = Color.black;
                stText.alignment = TextAlignmentOptions.Center;
                stText.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 50);
                
                sBtn.onClick.AddListener(() => {
                    GameState.currentPlayingStage = stageNum;
                    if (GridManager.Instance != null)
                        GameState.SaveGrid(GridManager.Instance.GetAllBuildings());
                    SceneManager.LoadScene("Defense");
                });
            }
        }
    }
    
    private void CreateDefenseEndGameUI()
    {
        var canvas = transform.parent;
        
        _endGameContainer = new GameObject("EndGameContainer");
        _endGameContainer.transform.SetParent(canvas, false);
        var img = _endGameContainer.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);
        var egRt = _endGameContainer.GetComponent<RectTransform>();
        egRt.anchorMin = Vector2.zero;
        egRt.anchorMax = Vector2.one;
        egRt.offsetMin = Vector2.zero;
        egRt.offsetMax = Vector2.zero;
        
        var resultTextGo = new GameObject("ResultText");
        resultTextGo.transform.SetParent(_endGameContainer.transform, false);
        var resultText = resultTextGo.AddComponent<TextMeshProUGUI>();
        resultText.text = "Result";
        resultText.fontSize = 60;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.color = Color.white;
        var rRt = resultTextGo.GetComponent<RectTransform>();
        rRt.anchoredPosition = new Vector2(0, 50);
        rRt.sizeDelta = new Vector2(600, 100);
        
        var retBtnGo = new GameObject("ReturnBtn");
        retBtnGo.transform.SetParent(_endGameContainer.transform, false);
        retBtnGo.AddComponent<Image>().color = Color.white;
        var retBtn = retBtnGo.AddComponent<Button>();
        var rbRt = retBtnGo.GetComponent<RectTransform>();
        rbRt.anchoredPosition = new Vector2(0, -50);
        rbRt.sizeDelta = new Vector2(200, 60);
        
        var retTextGo = new GameObject("Text");
        retTextGo.transform.SetParent(retBtnGo.transform, false);
        var retText = retTextGo.AddComponent<TextMeshProUGUI>();
        retText.text = "Return to Lobby";
        retText.color = Color.black;
        retText.alignment = TextAlignmentOptions.Center;
        retText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 60);
        
        retBtn.onClick.AddListener(() => {
            if (GridManager.Instance != null)
                GameState.SaveGrid(GridManager.Instance.GetAllBuildings());
            SceneManager.LoadScene("Lobby");
        });
        
        _endGameContainer.SetActive(false);
    }
    
    public void ShowEndGame(bool isWin, int rewardGold = 0)
    {
        if (_endGameContainer != null)
        {
            _endGameContainer.SetActive(true);
            var tmp = _endGameContainer.transform.Find("ResultText").GetComponent<TextMeshProUGUI>();
            if (isWin)
            {
                tmp.text = $"STAGE CLEAR\n<size=30>Reward: {rewardGold} G</size>";
                tmp.color = Color.green;
            }
            else
            {
                tmp.text = "STAGE FAILED";
                tmp.color = Color.red;
            }
        }
    }
    
    void Update()
    {
        bool isLobby = SceneManager.GetActiveScene().name == "Lobby";
        

        
        if (topBarText != null)
        {
            if (isLobby)
                topBarText.text = $"Gold: {GameState.currentGold}";
            else
                topBarText.text = "";
        }
            
        if (isLobby && BuildManager.Instance != null)
        {
            if (cancelButton != null) cancelButton.gameObject.SetActive(BuildManager.Instance.IsPlacing);
            
            if (BuildManager.Instance.isBuildModeActive)
            {
                if (!_buttonsCreated && BuildManager.Instance.availableBuildings != null && BuildManager.Instance.availableBuildings.Count > 0)
                {
                    CreateBuildButtons();
                    _buttonsCreated = true;
                }
                if (buttonsContainer != null) buttonsContainer.gameObject.SetActive(true);
                UpdateBuildButtons();
                if (_stageContainer != null) _stageContainer.SetActive(false);
            }
            else
            {
                if (buttonsContainer != null) buttonsContainer.gameObject.SetActive(false);
                if (_stageContainer != null) _stageContainer.SetActive(true);
            }
        }
        else
        {
            if (cancelButton != null) cancelButton.gameObject.SetActive(false);
            if (buttonsContainer != null) buttonsContainer.gameObject.SetActive(false);
        }
        
        UpdateSelectionUI();
    }
    
    private void UpdateSelectionUI()
    {
        var gridRenderer = Object.FindObjectOfType<GridRenderer>();
        if (gridRenderer != null && gridRenderer.SelectedBuilding != null)
        {
            var selected = gridRenderer.SelectedBuilding;
            if (selected.data.buildingType != BuildingType.Mine && selected.data.buildingType != BuildingType.Entrance)
            {
                _sellButton.gameObject.SetActive(true);
            }
            else
            {
                _sellButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_sellButton != null) _sellButton.gameObject.SetActive(false);
        }
    }
    
    private void CreateBuildButtons()
    {
        if (buttonsContainer == null) return;
        
        var hLayout = buttonsContainer.GetComponent<HorizontalLayoutGroup>();
        if (hLayout != null) DestroyImmediate(hLayout);
        
        var vLayout = buttonsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.spacing = 10;
        vLayout.childControlHeight = false;
        vLayout.childControlWidth = false;
        vLayout.childForceExpandHeight = false;
        vLayout.childForceExpandWidth = false;
        
        var containerRt = buttonsContainer.GetComponent<RectTransform>();
        containerRt.sizeDelta = new Vector2(1100, 170);
        
        var villageRow = new GameObject("VillageRow");
        villageRow.transform.SetParent(buttonsContainer, false);
        var villageLayout = villageRow.AddComponent<HorizontalLayoutGroup>();
        villageLayout.childAlignment = TextAnchor.MiddleCenter;
        villageLayout.spacing = 10;
        villageLayout.childControlHeight = false;
        villageLayout.childControlWidth = false;
        villageLayout.childForceExpandHeight = false;
        villageLayout.childForceExpandWidth = false;
        var vRt = villageRow.GetComponent<RectTransform>();
        vRt.sizeDelta = new Vector2(1100, 80);
        
        var towerRow = new GameObject("TowerRow");
        towerRow.transform.SetParent(buttonsContainer, false);
        var towerLayout = towerRow.AddComponent<HorizontalLayoutGroup>();
        towerLayout.childAlignment = TextAnchor.MiddleCenter;
        towerLayout.spacing = 10;
        towerLayout.childControlHeight = false;
        towerLayout.childControlWidth = false;
        towerLayout.childForceExpandHeight = false;
        towerLayout.childForceExpandWidth = false;
        var tRt = towerRow.GetComponent<RectTransform>();
        tRt.sizeDelta = new Vector2(1100, 80);
        
        foreach (var data in BuildManager.Instance.availableBuildings)
        {
            var btnGo = new GameObject($"Btn_{data.buildingName}");
            bool isTower = data.buildingType == BuildingType.Tower || data.buildingType == BuildingType.TowerAttackBuff;
            btnGo.transform.SetParent(isTower ? towerRow.transform : villageRow.transform, false);
            
            btnGo.AddComponent<Image>().color = Color.white;
            var btn = btnGo.AddComponent<Button>();
            
            var rt = btnGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 80);
            
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            var textMesh = textGo.AddComponent<TextMeshProUGUI>();
            textMesh.text = $"{data.buildingName}\n{data.cost}G";
            textMesh.color = Color.black;
            textMesh.fontSize = 32;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            
            var capturedData = data; 
            btn.onClick.AddListener(() => {
                if (BuildManager.Instance.GetRemainingCount(capturedData.buildingName) > 0)
                    BuildManager.Instance.StartPlacement(capturedData);
            });
        }
    }
    
    private void UpdateBuildButtons()
    {
        if (buttonsContainer == null) return;
        var buttons = buttonsContainer.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            var child = btn.transform;
            if (!child.name.StartsWith("Btn_")) continue;
            
            string bName = child.name.Replace("Btn_", "");
            int remain = BuildManager.Instance.GetRemainingCount(bName);
            var txt = child.GetComponentInChildren<TextMeshProUGUI>();
            
            if (txt != null)
            {
                var bData = BuildManager.Instance.availableBuildings.FirstOrDefault(b => b.buildingName == bName);
                if (bData != null)
                {
                    txt.text = $"{bData.buildingName}\n{bData.cost}G / X{remain}";
                }
            }
            
            btn.interactable = remain > 0;
            child.GetComponent<Image>().color = remain > 0 ? Color.white : new Color(0.8f, 0.8f, 0.8f);
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
            GameState.currentGold += refund;
            GridManager.Instance.RemoveBuilding(selected.id);
            gridRenderer.SelectedBuilding = null;
        }
    }
}
