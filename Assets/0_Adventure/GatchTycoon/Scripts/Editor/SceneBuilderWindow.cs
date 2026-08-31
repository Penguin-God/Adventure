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
            cam.transform.position = new Vector3(2, 5, -3);
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
            
            var goldTextGo = new GameObject("GoldText");
            goldTextGo.transform.SetParent(canvasGo.transform, false);
            var goldText = goldTextGo.AddComponent<TextMeshProUGUI>();
            goldText.text = "Gold: 0";
            goldText.fontSize = 36;
            goldText.alignment = TextAlignmentOptions.TopLeft;
            var rt = goldText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(20, -20);
            rt.sizeDelta = new Vector2(300, 50);
            uiMgr.goldText = goldText;
            
            var gachaBtnGo = new GameObject("GachaButton");
            gachaBtnGo.transform.SetParent(canvasGo.transform, false);
            gachaBtnGo.AddComponent<Image>().color = Color.white;
            var gachaBtn = gachaBtnGo.AddComponent<Button>();
            var btnRt = gachaBtnGo.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.5f, 0);
            btnRt.anchorMax = new Vector2(0.5f, 0);
            btnRt.pivot = new Vector2(0.5f, 0);
            btnRt.anchoredPosition = new Vector2(0, 20);
            btnRt.sizeDelta = new Vector2(200, 80);
            
            var gachaTextGo = new GameObject("Text");
            gachaTextGo.transform.SetParent(gachaBtnGo.transform, false);
            var gachaText = gachaTextGo.AddComponent<TextMeshProUGUI>();
            gachaText.text = "Draw Building (100G)";
            gachaText.color = Color.black;
            gachaText.alignment = TextAlignmentOptions.Center;
            gachaText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            uiMgr.gachaButton = gachaBtn;
            
            var upgBtnGo = new GameObject("UpgradeButton");
            upgBtnGo.transform.SetParent(canvasGo.transform, false);
            upgBtnGo.AddComponent<Image>().color = Color.green;
            var upgBtn = upgBtnGo.AddComponent<Button>();
            var upgRt = upgBtnGo.GetComponent<RectTransform>();
            upgRt.anchorMin = new Vector2(1, 0);
            upgRt.anchorMax = new Vector2(1, 0);
            upgRt.pivot = new Vector2(1, 0);
            upgRt.anchoredPosition = new Vector2(-20, 20);
            upgRt.sizeDelta = new Vector2(200, 80);
            
            var upgTextGo = new GameObject("Text");
            upgTextGo.transform.SetParent(upgBtnGo.transform, false);
            var upgText = upgTextGo.AddComponent<TextMeshProUGUI>();
            upgText.text = "Upgrade City Hall";
            upgText.color = Color.black;
            upgText.alignment = TextAlignmentOptions.Center;
            upgText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            uiMgr.upgradeCityHallButton = upgBtn;
            
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
                    new CityHallLevelInfo { level = 1, gridSizeX = 2, gridSizeY = 2, upgradeCost = 500, costCurrency = CurrencyType.Gold },
                    new CityHallLevelInfo { level = 2, gridSizeX = 3, gridSizeY = 3, upgradeCost = 2000, costCurrency = CurrencyType.Gold }
                };
                config.gachaCost = 100;
                config.gachaCurrency = CurrencyType.Gold;
                
                var cityHallData = ScriptableObject.CreateInstance<BuildingDataSO>();
                cityHallData.buildingName = "City Hall";
                cityHallData.category = BuildingCategory.CityHall;
                cityHallData.level = 1;
                cityHallData.goldPerHour = 10;
                AssetDatabase.CreateAsset(cityHallData, "Assets/0_Adventure/GatchTycoon/Resources/CityHallData.asset");
                
                var residence1 = ScriptableObject.CreateInstance<BuildingDataSO>();
                residence1.buildingName = "Goshiwon";
                residence1.category = BuildingCategory.Residence;
                residence1.level = 1;
                residence1.capacity = 1;
                residence1.baseOccupancyRate = 0.5f;
                AssetDatabase.CreateAsset(residence1, "Assets/0_Adventure/GatchTycoon/Resources/Residence1.asset");
                
                var residence2 = ScriptableObject.CreateInstance<BuildingDataSO>();
                residence2.buildingName = "OneRoom";
                residence2.category = BuildingCategory.Residence;
                residence2.level = 2;
                residence2.capacity = 2;
                residence2.baseOccupancyRate = 0.6f;
                AssetDatabase.CreateAsset(residence2, "Assets/0_Adventure/GatchTycoon/Resources/Residence2.asset");
                
                residence1.nextLevelBuilding = residence2;
                EditorUtility.SetDirty(residence1);
                
                var work1 = ScriptableObject.CreateInstance<BuildingDataSO>();
                work1.buildingName = "Startup";
                work1.category = BuildingCategory.Work;
                work1.level = 1;
                work1.capacity = 1;
                work1.goldPerHour = 20;
                AssetDatabase.CreateAsset(work1, "Assets/0_Adventure/GatchTycoon/Resources/Work1.asset");
                
                var work2 = ScriptableObject.CreateInstance<BuildingDataSO>();
                work2.buildingName = "Commercial House";
                work2.category = BuildingCategory.Work;
                work2.level = 2;
                work2.capacity = 2;
                work2.goldPerHour = 50;
                AssetDatabase.CreateAsset(work2, "Assets/0_Adventure/GatchTycoon/Resources/Work2.asset");
                
                work1.nextLevelBuilding = work2;
                EditorUtility.SetDirty(work1);
                
                config.gachaPool = new List<BuildingDataSO> { residence1, work1 };
                
                AssetDatabase.CreateAsset(config, "Assets/0_Adventure/GatchTycoon/Resources/GameConfig.asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                // Force update existing assets in case they were generated without nextLevelBuilding
                var r1 = AssetDatabase.LoadAssetAtPath<BuildingDataSO>("Assets/0_Adventure/GatchTycoon/Resources/Residence1.asset");
                var r2 = AssetDatabase.LoadAssetAtPath<BuildingDataSO>("Assets/0_Adventure/GatchTycoon/Resources/Residence2.asset");
                if (r1 != null && r2 != null && r1.nextLevelBuilding == null) { r1.nextLevelBuilding = r2; EditorUtility.SetDirty(r1); }
                
                var w1 = AssetDatabase.LoadAssetAtPath<BuildingDataSO>("Assets/0_Adventure/GatchTycoon/Resources/Work1.asset");
                var w2 = AssetDatabase.LoadAssetAtPath<BuildingDataSO>("Assets/0_Adventure/GatchTycoon/Resources/Work2.asset");
                if (w1 != null && w2 == null)
                {
                    w2 = ScriptableObject.CreateInstance<BuildingDataSO>();
                    w2.buildingName = "Commercial House";
                    w2.category = BuildingCategory.Work;
                    w2.level = 2;
                    w2.capacity = 2;
                    w2.goldPerHour = 50;
                    AssetDatabase.CreateAsset(w2, "Assets/0_Adventure/GatchTycoon/Resources/Work2.asset");
                    w1.nextLevelBuilding = w2;
                    EditorUtility.SetDirty(w1);
                }
                AssetDatabase.SaveAssets();
            }
            return config;
        }
    }
}
