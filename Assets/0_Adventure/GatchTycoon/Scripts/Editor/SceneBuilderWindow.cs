using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using GatchTycoon.Managers;
using GatchTycoon.UI;
using GatchTycoon.Data;
using GatchTycoon.Domain;
using System.Collections.Generic;

namespace GatchTycoon.Editor
{
    public class SceneBuilderWindow : EditorWindow
    {
        [MenuItem("GachaTycoon/Setup Scene")]
        public static void SetupScene()
        {
            var gameConfig = CreateOrLoadGameConfig();
            
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
            }
            cam.transform.position = new Vector3(3, 8, -4);
            cam.transform.rotation = Quaternion.Euler(60, 0, 0);
            
            var managers = new GameObject("Managers");
            var currencyMgr = managers.AddComponent<CurrencyManager>();
            var gridMgr = managers.AddComponent<GridManager>();
            var gachaMgr = managers.AddComponent<GachaManager>();
            var goldGen = managers.AddComponent<GoldGenerator>();
            
            gridMgr.gameConfig = gameConfig;
            
            var env = new GameObject("Environment");
            var gridRenderer = env.AddComponent<GridRenderer>();
            
            var tilesParent = new GameObject("Tiles").transform;
            tilesParent.SetParent(env.transform);
            var buildingsParent = new GameObject("Buildings").transform;
            buildingsParent.SetParent(env.transform);
            
            gridRenderer.tilesParent = tilesParent;
            gridRenderer.buildingsParent = buildingsParent;
            gridRenderer.tileSize = 1.2f;
            
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            
            var uiMgrGo = new GameObject("UIManager");
            uiMgrGo.transform.SetParent(canvasGo.transform, false);
            var uiMgr = uiMgrGo.AddComponent<UIManager>();
            
            // Top Bar
            var topBarGo = new GameObject("TopBarText");
            topBarGo.transform.SetParent(canvasGo.transform, false);
            var topBarText = topBarGo.AddComponent<TextMeshProUGUI>();
            topBarText.text = "Gold: 0 | Residents: 0/0 | Workers: 0/0";
            topBarText.fontSize = 24;
            topBarText.alignment = TextAlignmentOptions.TopLeft;
            var rt = topBarGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -20);
            rt.sizeDelta = new Vector2(-40, 50);
            uiMgr.topBarText = topBarText;
            
            // Gacha Button
            var gachaBtnGo = new GameObject("GachaButton");
            gachaBtnGo.transform.SetParent(canvasGo.transform, false);
            gachaBtnGo.AddComponent<Image>().color = Color.white;
            var gachaBtn = gachaBtnGo.AddComponent<Button>();
            var btnRt = gachaBtnGo.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.3f, 0);
            btnRt.anchorMax = new Vector2(0.3f, 0);
            btnRt.pivot = new Vector2(0.5f, 0);
            btnRt.anchoredPosition = new Vector2(0, 20);
            btnRt.sizeDelta = new Vector2(200, 80);
            var gachaTextGo = new GameObject("Text");
            gachaTextGo.transform.SetParent(gachaBtnGo.transform, false);
            var gachaText = gachaTextGo.AddComponent<TextMeshProUGUI>();
            gachaText.text = "Draw (100G)";
            gachaText.color = Color.black;
            gachaText.alignment = TextAlignmentOptions.Center;
            gachaText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            uiMgr.gachaButton = gachaBtn;
            
            // Combine Button
            var combineBtnGo = new GameObject("CombineButton");
            combineBtnGo.transform.SetParent(canvasGo.transform, false);
            combineBtnGo.AddComponent<Image>().color = Color.yellow;
            var combineBtn = combineBtnGo.AddComponent<Button>();
            var cbtnRt = combineBtnGo.GetComponent<RectTransform>();
            cbtnRt.anchorMin = new Vector2(0.7f, 0);
            cbtnRt.anchorMax = new Vector2(0.7f, 0);
            cbtnRt.pivot = new Vector2(0.5f, 0);
            cbtnRt.anchoredPosition = new Vector2(0, 20);
            cbtnRt.sizeDelta = new Vector2(200, 80);
            var combineTextGo = new GameObject("Text");
            combineTextGo.transform.SetParent(combineBtnGo.transform, false);
            var combineText = combineTextGo.AddComponent<TextMeshProUGUI>();
            combineText.text = "Combine UI";
            combineText.color = Color.black;
            combineText.alignment = TextAlignmentOptions.Center;
            combineText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            uiMgr.combineUIButton = combineBtn;
            
            // Upgrade City Hall Button
            var upgBtnGo = new GameObject("UpgradeButton");
            upgBtnGo.transform.SetParent(canvasGo.transform, false);
            upgBtnGo.AddComponent<Image>().color = Color.green;
            var upgBtn = upgBtnGo.AddComponent<Button>();
            var upgRt = upgBtnGo.GetComponent<RectTransform>();
            upgRt.anchorMin = new Vector2(1, 0);
            upgRt.anchorMax = new Vector2(1, 0);
            upgRt.pivot = new Vector2(1, 0);
            upgRt.anchoredPosition = new Vector2(-120, 20);
            upgRt.sizeDelta = new Vector2(200, 80);
            var upgTextGo = new GameObject("Text");
            upgTextGo.transform.SetParent(upgBtnGo.transform, false);
            var upgText = upgTextGo.AddComponent<TextMeshProUGUI>();
            upgText.text = "Upgrade City Hall";
            upgText.color = Color.black;
            upgText.alignment = TextAlignmentOptions.Center;
            upgText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            uiMgr.upgradeCityHallButton = upgBtn;
            
            // Combine Popup
            var combinePopupGo = new GameObject("CombinePopup");
            combinePopupGo.transform.SetParent(canvasGo.transform, false);
            combinePopupGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            var prt = combinePopupGo.GetComponent<RectTransform>();
            prt.anchorMin = new Vector2(0.1f, 0.1f);
            prt.anchorMax = new Vector2(0.9f, 0.9f);
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;
            
            var contentParentGo = new GameObject("Content");
            contentParentGo.transform.SetParent(combinePopupGo.transform, false);
            var layout = contentParentGo.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.spacing = 10;
            var crt = contentParentGo.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 0);
            crt.anchorMax = new Vector2(1, 1);
            crt.offsetMin = new Vector2(20, 20);
            crt.offsetMax = new Vector2(-20, -60);
            
            var closeBtnGo = new GameObject("CloseButton");
            closeBtnGo.transform.SetParent(combinePopupGo.transform, false);
            closeBtnGo.AddComponent<Image>().color = Color.red;
            var closeBtn = closeBtnGo.AddComponent<Button>();
            var clsRt = closeBtnGo.GetComponent<RectTransform>();
            clsRt.anchorMin = new Vector2(1, 1);
            clsRt.anchorMax = new Vector2(1, 1);
            clsRt.pivot = new Vector2(1, 1);
            clsRt.anchoredPosition = new Vector2(-10, -10);
            clsRt.sizeDelta = new Vector2(100, 40);
            var clsTextGo = new GameObject("Text");
            clsTextGo.transform.SetParent(closeBtnGo.transform, false);
            var clsText = clsTextGo.AddComponent<TextMeshProUGUI>();
            clsText.text = "Close";
            clsText.color = Color.white;
            clsText.alignment = TextAlignmentOptions.Center;
            clsText.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);
            
            var recipePrefabGo = new GameObject("RecipeItemPrefab");
            recipePrefabGo.AddComponent<Image>().color = Color.gray;
            var reqRt = recipePrefabGo.GetComponent<RectTransform>();
            reqRt.sizeDelta = new Vector2(600, 60);
            var recTitle = new GameObject("TitleText");
            recTitle.transform.SetParent(recipePrefabGo.transform, false);
            var ttmp = recTitle.AddComponent<TextMeshProUGUI>();
            ttmp.text = "Recipe";
            ttmp.alignment = TextAlignmentOptions.Left;
            var trt = recTitle.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0);
            trt.anchorMax = new Vector2(1, 1);
            trt.offsetMin = new Vector2(10, 0);
            
            var recBtnGo = new GameObject("CombineBtn");
            recBtnGo.transform.SetParent(recipePrefabGo.transform, false);
            recBtnGo.AddComponent<Image>().color = Color.green;
            var recBtn = recBtnGo.AddComponent<Button>();
            var rbrt = recBtnGo.GetComponent<RectTransform>();
            rbrt.anchorMin = new Vector2(1, 0.5f);
            rbrt.anchorMax = new Vector2(1, 0.5f);
            rbrt.pivot = new Vector2(1, 0.5f);
            rbrt.anchoredPosition = new Vector2(-10, 0);
            rbrt.sizeDelta = new Vector2(120, 40);
            var rbText = new GameObject("Text");
            rbText.transform.SetParent(recBtnGo.transform, false);
            var rbtmp = rbText.AddComponent<TextMeshProUGUI>();
            rbtmp.text = "Combine";
            rbtmp.color = Color.black;
            rbtmp.alignment = TextAlignmentOptions.Center;
            rbtmp.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 40);
            
            // Convert to real prefab
            if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/GatchTycoon/Resources/UI"))
            {
                AssetDatabase.CreateFolder("Assets/0_Adventure/GatchTycoon/Resources", "UI");
            }
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(recipePrefabGo, "Assets/0_Adventure/GatchTycoon/Resources/UI/RecipeItemPrefab.prefab");
            DestroyImmediate(recipePrefabGo);
            
            var popupUI = combinePopupGo.AddComponent<CombinePopupUI>();
            popupUI.contentParent = contentParentGo.transform;
            popupUI.recipeItemPrefab = savedPrefab;
            popupUI.closeButton = closeBtn;
            
            combinePopupGo.SetActive(false);
            uiMgr.combinePopup = combinePopupGo;
            
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            var initScriptGo = new GameObject("GameInitializer");
            initScriptGo.AddComponent<GameInitializer>();
            
            Debug.Log("Gacha Tycoon Scene setup complete!");
        }
        
        private static GameConfigSO CreateOrLoadGameConfig()
        {
            if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/GatchTycoon/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/0_Adventure/GatchTycoon", "Resources");
            }
            
            var config = AssetDatabase.LoadAssetAtPath<GameConfigSO>("Assets/0_Adventure/GatchTycoon/Resources/GameConfig.asset");
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<GameConfigSO>();
                
                config.cityHallLevels = new List<CityHallLevelInfo>
                {
                    new CityHallLevelInfo { level = 1, gridSizeX = 3, gridSizeY = 3, upgradeCost = 500, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 2, gridSizeX = 4, gridSizeY = 4, upgradeCost = 2000, costCurrency = CurrencyType.Gold }
                };
                config.gachaCost = 100;
                config.gachaCurrency = CurrencyType.Gold;
                
                var cityHallData = ScriptableObject.CreateInstance<BuildingDataSO>();
                cityHallData.buildingName = "City Hall";
                cityHallData.category = BuildingCategory.CityHall;
                cityHallData.level = 1;
                cityHallData.baseGoldPerHour = 50;
                AssetDatabase.CreateAsset(cityHallData, "Assets/0_Adventure/GatchTycoon/Resources/CityHallData.asset");
                
                // Residences
                var res3 = CreateBuilding("Apartment", BuildingCategory.Residence, 3, 50, 10, 4);
                var res2 = CreateBuilding("Villa", BuildingCategory.Residence, 2, 20, 5, 3, res3);
                var res1 = CreateBuilding("OneRoom", BuildingCategory.Residence, 1, 10, 2, 2, res2);
                
                // Works
                var work3 = CreateBuilding("Corporate Building", BuildingCategory.Work, 3, 100, 0, 0, null, 10, 20);
                var work2 = CreateBuilding("Officetel", BuildingCategory.Work, 2, 40, 0, 0, work3, 5, 10);
                var work1 = CreateBuilding("Startup", BuildingCategory.Work, 1, 15, 0, 0, work2, 2, 5);
                
                // Convenience
                var conv3 = CreateBuilding("Shopping Mall", BuildingCategory.Convenience, 3, 30, 0, 0, null, 0, 0, 3, BuffType.MoneyEfficiency, 1.0f);
                var conv2 = CreateBuilding("Mart", BuildingCategory.Convenience, 2, 10, 0, 0, conv3, 0, 0, 2, BuffType.OccupancyRate, 0.5f);
                var conv1 = CreateBuilding("Restaurant", BuildingCategory.Convenience, 1, 5, 0, 0, conv2, 0, 0, 1, BuffType.OccupancyRate, 0.2f);
                
                config.gachaPool = new List<BuildingDataSO> { res1, work1, conv1 };
                
                AssetDatabase.CreateAsset(config, "Assets/0_Adventure/GatchTycoon/Resources/GameConfig.asset");
                AssetDatabase.SaveAssets();
            }
            return config;
        }
        
        private static BuildingDataSO CreateBuilding(string name, BuildingCategory cat, int level, int baseGold, 
            int capacity = 0, int commute = 0, BuildingDataSO nextLvl = null, 
            int totalJobs = 0, int profit = 0, int buffRange = 0, BuffType buffType = BuffType.OccupancyRate, float buffAmt = 0)
        {
            var b = ScriptableObject.CreateInstance<BuildingDataSO>();
            b.buildingName = name;
            b.category = cat;
            b.level = level;
            b.baseGoldPerHour = baseGold;
            b.capacity = capacity;
            b.commuteRange = commute;
            b.totalJobs = totalJobs;
            b.profitPerWorker = profit;
            b.buffRange = buffRange;
            b.buffType = buffType;
            b.buffAmount = buffAmt;
            b.nextLevelBuilding = nextLvl;
            b.requiredCount = 3;
            
            AssetDatabase.CreateAsset(b, $"Assets/0_Adventure/GatchTycoon/Resources/{name.Replace(" ", "")}.asset");
            return b;
        }
    }
}
