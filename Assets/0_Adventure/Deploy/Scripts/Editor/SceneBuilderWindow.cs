using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SceneBuilderWindow : EditorWindow
{
    [MenuItem("Adventure/Build Scenes")]
    public static void BuildScenes()
    {
        GenerateScriptableObjects();
        
        // Build Lobby Scene
        var lobbyScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        lobbyScene.name = "Lobby";
        BuildLobbyScene();
        EditorSceneManager.SaveScene(lobbyScene, "Assets/0_Adventure/Deploy/Lobby.unity");
        
        // Build Defense Scene
        var defenseScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        defenseScene.name = "Defense";
        BuildDefenseScene();
        EditorSceneManager.SaveScene(defenseScene, "Assets/0_Adventure/Deploy/Defense.unity");
        
        // Update Build Settings
        var scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/0_Adventure/Deploy/Lobby.unity", true),
            new EditorBuildSettingsScene("Assets/0_Adventure/Deploy/Defense.unity", true)
        };
        EditorBuildSettings.scenes = scenes.ToArray();
        
        // Load Lobby as start
        EditorSceneManager.OpenScene("Assets/0_Adventure/Deploy/Lobby.unity");
        
        Debug.Log("Lobby and Defense scenes created successfully!");
    }
    
    private static void BuildLobbyScene()
    {
        var mainCam = new GameObject("Main Camera");
        mainCam.tag = "MainCamera";
        var cam = mainCam.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 9f;
        mainCam.transform.position = new Vector3(7f, 0f, -10f);
        
        var gridManagerGo = new GameObject("GridManager");
        var gridManager = gridManagerGo.AddComponent<GridManager>();
        
        var gridRendererGo = new GameObject("GridRenderer");
        var gridRenderer = gridRendererGo.AddComponent<GridRenderer>();
        var tilesGo = new GameObject("Tiles");
        var buildingsGo = new GameObject("Buildings");
        tilesGo.transform.SetParent(gridRendererGo.transform);
        buildingsGo.transform.SetParent(gridRendererGo.transform);
        gridRenderer.tilesParent = tilesGo.transform;
        gridRenderer.buildingsParent = buildingsGo.transform;
        
        var buildManagerGo = new GameObject("BuildManager");
        var buildManager = buildManagerGo.AddComponent<BuildManager>();
        
        var supplyChainGo = new GameObject("SupplyChainManager");
        var supplyChain = supplyChainGo.AddComponent<SupplyChainManager>();
        
        var uiManagerGo = new GameObject("UIManager");
        var uiManager = uiManagerGo.AddComponent<UIManager>();
        
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        uiManagerGo.transform.SetParent(canvasGo.transform);
        
        // Top Bar
        var topBarGo = new GameObject("TopBarText");
        topBarGo.transform.SetParent(canvasGo.transform, false);
        var topBarText = topBarGo.AddComponent<TextMeshProUGUI>();
        topBarText.alignment = TextAlignmentOptions.Center;
        topBarText.fontSize = 24;
        topBarText.color = Color.black;
        var topBarRt = topBarGo.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0, 1);
        topBarRt.anchorMax = new Vector2(1, 1);
        topBarRt.pivot = new Vector2(0.5f, 1);
        topBarRt.anchoredPosition = new Vector2(0, -20);
        topBarRt.sizeDelta = new Vector2(-40, 50);
        uiManager.topBarText = topBarText;
        
        // Build Mode Buttons Container
        var buttonsContainerGo = new GameObject("ButtonsContainer");
        buttonsContainerGo.transform.SetParent(canvasGo.transform, false);
        var containerRt = buttonsContainerGo.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0, 0);
        containerRt.anchorMax = new Vector2(1, 0);
        containerRt.pivot = new Vector2(0.5f, 0);
        containerRt.anchoredPosition = new Vector2(0, 20);
        containerRt.sizeDelta = new Vector2(-100, 80);
        var layoutGroup = buttonsContainerGo.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 10;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        uiManager.buttonsContainer = buttonsContainerGo.transform;
        
        // Stage Container
        var stageContainerGo = new GameObject("StageContainer");
        stageContainerGo.transform.SetParent(canvasGo.transform, false);
        var stageContainerRt = stageContainerGo.AddComponent<RectTransform>();
        stageContainerRt.anchorMin = new Vector2(0.5f, 0.5f);
        stageContainerRt.anchorMax = new Vector2(0.5f, 0.5f);
        stageContainerRt.pivot = new Vector2(0.5f, 0.5f);
        stageContainerRt.anchoredPosition = new Vector2(0, 250);
        stageContainerRt.sizeDelta = new Vector2(600, 100);
        var stageLayout = stageContainerGo.AddComponent<HorizontalLayoutGroup>();
        stageLayout.childAlignment = TextAnchor.MiddleCenter;
        stageLayout.spacing = 20;
        
        // Cancel Button
        var cancelBtnGo = new GameObject("CancelButton");
        cancelBtnGo.transform.SetParent(canvasGo.transform, false);
        cancelBtnGo.AddComponent<Image>().color = new Color(1f, 0.4f, 0.4f);
        var cancelBtn = cancelBtnGo.AddComponent<Button>();
        var cancelRt = cancelBtnGo.GetComponent<RectTransform>();
        cancelRt.anchorMin = new Vector2(0.95f, 1);
        cancelRt.anchorMax = new Vector2(0.95f, 1);
        cancelRt.pivot = new Vector2(1f, 1);
        cancelRt.anchoredPosition = new Vector2(0, -20);
        cancelRt.sizeDelta = new Vector2(150, 50);
        var cancelTextGo = new GameObject("Text");
        cancelTextGo.transform.SetParent(cancelBtnGo.transform, false);
        var cancelText = cancelTextGo.AddComponent<TextMeshProUGUI>();
        cancelText.text = "Cancel";
        cancelText.color = Color.white;
        cancelText.alignment = TextAlignmentOptions.Center;
        cancelText.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 50);
        uiManager.cancelButton = cancelBtn;
        
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
    
    private static void BuildDefenseScene()
    {
        var mainCam = new GameObject("Main Camera");
        mainCam.tag = "MainCamera";
        var cam = mainCam.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 9f;
        mainCam.transform.position = new Vector3(7f, 0f, -10f);
        
        var gridManagerGo = new GameObject("GridManager");
        var gridManager = gridManagerGo.AddComponent<GridManager>();
        
        var gridRendererGo = new GameObject("GridRenderer");
        var gridRenderer = gridRendererGo.AddComponent<GridRenderer>();
        var tilesGo = new GameObject("Tiles");
        var buildingsGo = new GameObject("Buildings");
        tilesGo.transform.SetParent(gridRendererGo.transform);
        buildingsGo.transform.SetParent(gridRendererGo.transform);
        gridRenderer.tilesParent = tilesGo.transform;
        gridRenderer.buildingsParent = buildingsGo.transform;
        
        var supplyChainGo = new GameObject("SupplyChainManager");
        var supplyChain = supplyChainGo.AddComponent<SupplyChainManager>();
        
        var defenseManagerGo = new GameObject("DefenseManager");
        var defenseManager = defenseManagerGo.AddComponent<DefenseManager>();
        
        var monsterManagerGo = new GameObject("MonsterManager");
        var monsterManager = monsterManagerGo.AddComponent<MonsterManager>();
        
        var projectileManagerGo = new GameObject("ProjectileManager");
        var projectileManager = projectileManagerGo.AddComponent<ProjectileManager>();
        
        var uiManagerGo = new GameObject("UIManager");
        var uiManager = uiManagerGo.AddComponent<UIManager>();
        
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        uiManagerGo.transform.SetParent(canvasGo.transform);
        
        var topBarGo = new GameObject("TopBarText");
        topBarGo.transform.SetParent(canvasGo.transform, false);
        var topBarText = topBarGo.AddComponent<TextMeshProUGUI>();
        topBarText.alignment = TextAlignmentOptions.Center;
        topBarText.fontSize = 24;
        topBarText.color = Color.black;
        var topBarRt = topBarGo.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0, 1);
        topBarRt.anchorMax = new Vector2(1, 1);
        topBarRt.pivot = new Vector2(0.5f, 1);
        topBarRt.anchoredPosition = new Vector2(0, -20);
        topBarRt.sizeDelta = new Vector2(-40, 50);
        uiManager.topBarText = topBarText;
        
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
    
    private static void GenerateScriptableObjects()
    {
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources/Buildings"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy/Resources", "Buildings");
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources/Monsters"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy/Resources", "Monsters");
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources/Stages"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy/Resources", "Stages");
            
        // Mines
        CreateBuildingData("StoneMine", BuildingType.Mine, 100, 1, 0.1f, ResourceType.None, ResourceType.None, 0, ResourceType.Stone, 50);
        CreateBuildingData("IronMine", BuildingType.Mine, 100, 1, 0.1f, ResourceType.None, ResourceType.None, 0, ResourceType.Iron, 50);
        
        // Entrance & Road
        CreateBuildingData("Entrance", BuildingType.Entrance, 0, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0);
        CreateBuildingData("Road", BuildingType.Road, 100, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0);
        
        // Factories
        CreateBuildingData("StoneFactory", BuildingType.Factory, 100, 1, 1f, ResourceType.Stone, ResourceType.None, 5, ResourceType.RoundStone, 5);
        CreateBuildingData("ArrowFactory", BuildingType.Factory, 150, 1, 1f, ResourceType.Iron, ResourceType.None, 5, ResourceType.Arrow, 5);
        CreateBuildingData("AmmoFactory", BuildingType.Factory, 200, 1, 1f, ResourceType.Arrow, ResourceType.RoundStone, 5, ResourceType.GunAmmo, 5);
        
        // Buffs
        CreateBuildingData("FactoryBuff", BuildingType.FactorySpeedBuff, 100, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, buffAmt: 1f, buffRng: 1);
        CreateBuildingData("TowerBuff", BuildingType.TowerAttackBuff, 200, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, buffAmt: 0.5f, buffRng: 1);

        // Towers
        CreateBuildingData("Slingshot", BuildingType.Tower, 150, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, atk: 50f, atkSpd: 0.7f, atkRange: 6, maxAmmo: 3, reqAmmo: ResourceType.RoundStone);
        CreateBuildingData("Archer", BuildingType.Tower, 200, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, atk: 35f, atkSpd: 1.2f, atkRange: 10, maxAmmo: 5, reqAmmo: ResourceType.Arrow);
        CreateBuildingData("Gun", BuildingType.Tower, 300, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, atk: 50f, atkSpd: 2f, atkRange: 15, maxAmmo: 10, reqAmmo: ResourceType.GunAmmo);
        
        var monsterData = ScriptableObject.CreateInstance<MonsterDataSO>();
        monsterData.maxHp = 150f;
        monsterData.speed = 1f;
        monsterData.rewardGold = 20;
        AssetDatabase.CreateAsset(monsterData, "Assets/0_Adventure/Deploy/Resources/Monsters/BasicMonster.asset");
        
        var settingsPath = "Assets/0_Adventure/Deploy/Resources/GameSettings.asset";
        var settings = AssetDatabase.LoadAssetAtPath<GameSettingsSO>(settingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<GameSettingsSO>();
            AssetDatabase.CreateAsset(settings, settingsPath);
        }
        settings.initialWaitTime = 10f;
        settings.startingGold = 3000;
        settings.monsterSpawnDelay = 1f;
        settings.monsterBaseHp = 150f;
        settings.monsterHpIncreaseStep = 10;
        settings.monsterHpIncreasePercent = 0.1f;
        EditorUtility.SetDirty(settings);
        
        // Generate StageDataSO
        CreateStageData(1, 150f, 2.0f, 10, new List<BuildingCount> {
            new BuildingCount { buildingName = "StoneFactory", count = 3 },
            new BuildingCount { buildingName = "Road", count = 10 },
            new BuildingCount { buildingName = "Slingshot", count = 3 }
        });
        
        CreateStageData(2, 200f, 1.0f, 10, new List<BuildingCount> {
            new BuildingCount { buildingName = "Archer", count = 3 },
            new BuildingCount { buildingName = "ArrowFactory", count = 3 },
            new BuildingCount { buildingName = "Road", count = 5 }
        });
        
        CreateStageData(3, 300f, 1.0f, 15, new List<BuildingCount> {
            new BuildingCount { buildingName = "AmmoFactory", count = 3 },
            new BuildingCount { buildingName = "Road", count = 5 },
            new BuildingCount { buildingName = "Gun", count = 3 }
        });
        
        CreateStageData(4, 500f, 1.0f, 15, new List<BuildingCount> {
            new BuildingCount { buildingName = "TowerBuff", count = 3 },
            new BuildingCount { buildingName = "FactoryBuff", count = 3 }
        });
        
        CreateStageData(5, 1000f, 0.8f, 25, new List<BuildingCount> {
            new BuildingCount { buildingName = "StoneFactory", count = 3 },
            new BuildingCount { buildingName = "ArrowFactory", count = 3 },
            new BuildingCount { buildingName = "AmmoFactory", count = 3 },
            new BuildingCount { buildingName = "Slingshot", count = 2 },
            new BuildingCount { buildingName = "Archer", count = 2 },
            new BuildingCount { buildingName = "Gun", count = 2 }
        });
        
        AssetDatabase.SaveAssets();
    }
    
    private static void CreateStageData(int stageNum, float hp, float delay, int count, List<BuildingCount> rewards)
    {
        string path = $"Assets/0_Adventure/Deploy/Resources/Stages/Stage_{stageNum}.asset";
        var s = AssetDatabase.LoadAssetAtPath<StageDataSO>(path);
        if (s == null)
        {
            s = ScriptableObject.CreateInstance<StageDataSO>();
            AssetDatabase.CreateAsset(s, path);
        }
        s.stageNumber = stageNum;
        s.monsterHp = hp;
        s.spawnDelay = delay;
        s.totalMonsters = count;
        s.buildingRewards = rewards;
        EditorUtility.SetDirty(s);
    }
    
    private static void CreateBuildingData(
        string name, BuildingType type, int cost, int connRange, 
        float prodTime, ResourceType in1, ResourceType in2, int maxIn, 
        ResourceType outType, int maxOut, 
        float buffAmt = 0, int buffRng = 0, 
        float atk = 0, float atkSpd = 0, float atkRange = 0, int maxAmmo = 0, ResourceType reqAmmo = ResourceType.None)
    {
        string path = $"Assets/0_Adventure/Deploy/Resources/Buildings/{name}.asset";
        var b = AssetDatabase.LoadAssetAtPath<BuildingDataSO>(path);
        if (b == null)
        {
            b = ScriptableObject.CreateInstance<BuildingDataSO>();
            AssetDatabase.CreateAsset(b, path);
        }
        b.buildingName = name;
        b.buildingType = type;
        b.cost = cost;
        b.connectionRange = connRange;
        
        b.productionTime = prodTime;
        b.inputType1 = in1;
        b.inputType2 = in2;
        b.maxInputCapacity = maxIn;
        b.outputType = outType;
        b.maxOutputCapacity = maxOut;
        
        b.buffAmount = buffAmt;
        b.buffRange = buffRng;
        
        b.attackDamage = atk;
        b.attackSpeed = atkSpd;
        b.attackRange = atkRange;
        b.maxAmmo = maxAmmo;
        b.requiredAmmoType = reqAmmo;
        
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/0_Adventure/Deploy/Resources/Sprites/{name}.png");
        if (sprite != null) b.sprite = sprite;
        
        EditorUtility.SetDirty(b);
    }
}
