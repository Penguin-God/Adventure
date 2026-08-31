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
            goldGen.tickRate = 10f; // Every 10 seconds
            
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
            gachaText.text = "Draw (500G)";
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
            combinePopupGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.9f);
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
            layout.spacing = 15;
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
            recipePrefabGo.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var reqRt = recipePrefabGo.GetComponent<RectTransform>();
            reqRt.sizeDelta = new Vector2(800, 60); // Widened to prevent text overflow
            var recTitle = new GameObject("TitleText");
            recTitle.transform.SetParent(recipePrefabGo.transform, false);
            var ttmp = recTitle.AddComponent<TextMeshProUGUI>();
            ttmp.text = "Recipe Text";
            ttmp.fontSize = 20;
            ttmp.alignment = TextAlignmentOptions.Left;
            var trt = recTitle.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0);
            trt.anchorMax = new Vector2(1, 1);
            trt.offsetMin = new Vector2(10, 0);
            trt.offsetMax = new Vector2(-150, 0);
            
            var recBtnGo = new GameObject("CombineBtn");
            recBtnGo.transform.SetParent(recipePrefabGo.transform, false);
            recBtnGo.AddComponent<Image>().color = Color.green;
            var recBtn = recBtnGo.AddComponent<Button>();
            var rbrt = recBtnGo.GetComponent<RectTransform>();
            rbrt.anchorMin = new Vector2(1, 0.5f);
            rbrt.anchorMax = new Vector2(1, 0.5f);
            rbrt.pivot = new Vector2(1, 0.5f);
            rbrt.anchoredPosition = new Vector2(-10, 0);
            rbrt.sizeDelta = new Vector2(140, 40);
            var rbText = new GameObject("Text");
            rbText.transform.SetParent(recBtnGo.transform, false);
            var rbtmp = rbText.AddComponent<TextMeshProUGUI>();
            rbtmp.text = "Combine";
            rbtmp.color = Color.black;
            rbtmp.alignment = TextAlignmentOptions.Center;
            rbtmp.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 40);
            
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
            
            // Info Popup
            var infoPopupGo = new GameObject("InfoPopup");
            infoPopupGo.transform.SetParent(canvasGo.transform, false);
            infoPopupGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.9f);
            var iprt = infoPopupGo.GetComponent<RectTransform>();
            iprt.anchorMin = new Vector2(0.3f, 0.3f);
            iprt.anchorMax = new Vector2(0.7f, 0.7f);
            iprt.offsetMin = Vector2.zero;
            iprt.offsetMax = Vector2.zero;
            
            var infoTextGo = new GameObject("InfoText");
            infoTextGo.transform.SetParent(infoPopupGo.transform, false);
            var itmp = infoTextGo.AddComponent<TextMeshProUGUI>();
            itmp.text = "Building Info";
            itmp.fontSize = 24;
            itmp.alignment = TextAlignmentOptions.TopLeft;
            var itrt = infoTextGo.GetComponent<RectTransform>();
            itrt.anchorMin = new Vector2(0, 0);
            itrt.anchorMax = new Vector2(1, 1);
            itrt.offsetMin = new Vector2(20, -20);
            itrt.offsetMax = new Vector2(-20, -20);
            
            var infoCloseBtnGo = new GameObject("CloseButton");
            infoCloseBtnGo.transform.SetParent(infoPopupGo.transform, false);
            infoCloseBtnGo.AddComponent<Image>().color = Color.red;
            var infoCloseBtn = infoCloseBtnGo.AddComponent<Button>();
            var icrt = infoCloseBtnGo.GetComponent<RectTransform>();
            icrt.anchorMin = new Vector2(1, 1);
            icrt.anchorMax = new Vector2(1, 1);
            icrt.pivot = new Vector2(1, 1);
            icrt.anchoredPosition = new Vector2(-10, -10);
            icrt.sizeDelta = new Vector2(100, 40);
            var icTextGo = new GameObject("Text");
            icTextGo.transform.SetParent(infoCloseBtnGo.transform, false);
            var icText = icTextGo.AddComponent<TextMeshProUGUI>();
            icText.text = "Close";
            icText.color = Color.white;
            icText.alignment = TextAlignmentOptions.Center;
            icText.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);
            
            var infoUI = infoPopupGo.AddComponent<BuildingInfoPopupUI>();
            infoUI.infoText = itmp;
            infoUI.closeButton = infoCloseBtn;
            infoPopupGo.SetActive(false);
            uiMgr.infoPopup = infoUI;
            
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
                    new CityHallLevelInfo { level = 1, gridSizeX = 3, gridSizeY = 3, upgradeCost = 3000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 2, gridSizeX = 4, gridSizeY = 4, upgradeCost = 10000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 3, gridSizeX = 5, gridSizeY = 5, upgradeCost = 50000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 4, gridSizeX = 6, gridSizeY = 6, upgradeCost = 500000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 5, gridSizeX = 7, gridSizeY = 7, upgradeCost = 5000000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 6, gridSizeX = 8, gridSizeY = 8, upgradeCost = 50000000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 7, gridSizeX = 9, gridSizeY = 9, upgradeCost = 500000000, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 8, gridSizeX = 10, gridSizeY = 10, upgradeCost = -1, costCurrency = CurrencyType.Gold }
                };
                config.gachaCost = 500;
                config.gachaCurrency = CurrencyType.Gold;
                
                var cityHallData = ScriptableObject.CreateInstance<BuildingDataSO>();
                cityHallData.buildingName = "City Hall";
                cityHallData.category = BuildingCategory.CityHall;
                cityHallData.level = 1;
                cityHallData.baseGoldPerHour = 100; // Will be generated anyway
                AssetDatabase.CreateAsset(cityHallData, "Assets/0_Adventure/GatchTycoon/Resources/CityHallData.asset");
                
                // Residences
                var res3 = CreateBuilding("Apartment", BuildingCategory.Residence, 3, 1000, 50, 0.5f, RangePattern.Square3x3);
                var res2 = CreateBuilding("Villa", BuildingCategory.Residence, 2, 300, 20, 0.4f, RangePattern.Cross, res3, 5000);
                var res1 = CreateBuilding("OneRoom", BuildingCategory.Residence, 1, 100, 10, 0.3f, RangePattern.LeftRight, res2, 2000);
                
                // Works
                var work3 = CreateBuilding("Corporate Building", BuildingCategory.Work, 3, 1000, 0, 0f, RangePattern.None, null, 0, 50, 1000);
                var work2 = CreateBuilding("Officetel", BuildingCategory.Work, 2, 300, 0, 0f, RangePattern.None, work3, 5000, 20, 400);
                var work1 = CreateBuilding("Startup", BuildingCategory.Work, 1, 100, 0, 0f, RangePattern.None, work2, 2000, 10, 200);
                
                // Convenience (Money Efficiency)
                var conv3 = CreateBuilding("Shopping Mall", BuildingCategory.Convenience, 3, 1000, 0, 0f, RangePattern.Square3x3, null, 0, 0, 0, BuffType.MoneyEfficiency, 1.0f);
                var conv2 = CreateBuilding("Mart", BuildingCategory.Convenience, 2, 300, 0, 0f, RangePattern.Cross, conv3, 5000, 0, 0, BuffType.MoneyEfficiency, 0.5f);
                var conv1 = CreateBuilding("Cafe", BuildingCategory.Convenience, 1, 100, 0, 0f, RangePattern.LeftRight, conv2, 2000, 0, 0, BuffType.MoneyEfficiency, 0.2f);
                
                // Public (Occupancy Rate)
                var pub3 = CreateBuilding("Library", BuildingCategory.Public, 3, 1000, 0, 0f, RangePattern.Cross, null, 0, 0, 0, BuffType.OccupancyRate, 0.10f);
                var pub2 = CreateBuilding("Fire Station", BuildingCategory.Public, 2, 300, 0, 0f, RangePattern.AllDiagonals, pub3, 5000, 0, 0, BuffType.OccupancyRate, 0.05f);
                var pub1 = CreateBuilding("Police Station", BuildingCategory.Public, 1, 100, 0, 0f, RangePattern.TopDiagonals, pub2, 2000, 0, 0, BuffType.OccupancyRate, 0.05f);
                
                config.gachaPool = new List<BuildingDataSO> { res1, work1, conv1, pub1 };
                
                AssetDatabase.CreateAsset(config, "Assets/0_Adventure/GatchTycoon/Resources/GameConfig.asset");
                AssetDatabase.SaveAssets();
            }
            return config;
        }
        
        private static BuildingDataSO CreateBuilding(string name, BuildingCategory cat, int level, int baseGold, 
            int capacity = 0, float baseOccupancy = 0f, RangePattern pattern = RangePattern.None, BuildingDataSO nextLvl = null, 
            int combineCost = 0, int totalJobs = 0, int profit = 0, BuffType buffType = BuffType.OccupancyRate, float buffAmt = 0)
        {
            var b = ScriptableObject.CreateInstance<BuildingDataSO>();
            b.buildingName = name;
            b.category = cat;
            b.level = level;
            b.baseGoldPerHour = baseGold;
            b.capacity = capacity;
            b.baseOccupancyRate = baseOccupancy;
            if (cat == BuildingCategory.Residence) b.commutePattern = pattern;
            else b.effectPattern = pattern;
            b.totalJobs = totalJobs;
            b.profitPerWorker = profit;
            b.buffType = buffType;
            b.buffAmount = buffAmt;
            b.nextLevelBuilding = nextLvl;
            b.combineCost = combineCost;
            b.requiredCount = 3;
            
            AssetDatabase.CreateAsset(b, $"Assets/0_Adventure/GatchTycoon/Resources/{name.Replace(" ", "")}.asset");
            return b;
        }
    }
}
