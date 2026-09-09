using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SceneBuilderWindow : EditorWindow
{
    [MenuItem("Deploy/Setup Defense Scene")]
    public static void SetupScene()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraGo = new GameObject("Main Camera");
            camera = cameraGo.AddComponent<Camera>();
            cameraGo.tag = "MainCamera";
        }
        camera.orthographic = true;
        camera.orthographicSize = 8f;
        camera.transform.position = new Vector3(4.5f, 4.5f, -10f);
        camera.transform.rotation = Quaternion.identity;
        
        var managersParent = new GameObject("Managers");
        managersParent.AddComponent<GridManager>();
        managersParent.AddComponent<MonsterManager>();
        managersParent.AddComponent<DefenseManager>();
        managersParent.AddComponent<BuildManager>();
        managersParent.AddComponent<ProjectileManager>();
        managersParent.AddComponent<SupplyChainManager>();
        
        var environmentParent = new GameObject("Environment");
        var gridRenderer = environmentParent.AddComponent<GridRenderer>();
        
        var tilesParent = new GameObject("Tiles").transform;
        tilesParent.SetParent(environmentParent.transform);
        var buildingsParent = new GameObject("Buildings").transform;
        buildingsParent.SetParent(environmentParent.transform);
        var monstersParent = new GameObject("Monsters").transform;
        monstersParent.SetParent(environmentParent.transform);
        var projectilesParent = new GameObject("Projectiles").transform;
        projectilesParent.SetParent(environmentParent.transform);
        
        gridRenderer.tilesParent = tilesParent;
        gridRenderer.buildingsParent = buildingsParent;
        
        var monsterManager = managersParent.GetComponent<MonsterManager>();
        if(monsterManager != null) monsterManager.monstersParent = monstersParent;
        
        var projectileManager = managersParent.GetComponent<ProjectileManager>();
        if(projectileManager != null) projectileManager.projectilesParent = projectilesParent;
        
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        
        var uiManagerGo = new GameObject("UIManager");
        uiManagerGo.transform.SetParent(canvasGo.transform, false);
        var uiManager = uiManagerGo.AddComponent<UIManager>();
        
        // Top Bar
        var topBarGo = new GameObject("TopBarText");
        topBarGo.transform.SetParent(canvasGo.transform, false);
        var topBarText = topBarGo.AddComponent<TextMeshProUGUI>();
        topBarText.text = "Defense Gold: 0 | Wave: 1";
        topBarText.fontSize = 24;
        topBarText.alignment = TextAlignmentOptions.TopLeft;
        var topBarRt = topBarGo.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0, 1);
        topBarRt.anchorMax = new Vector2(1, 1);
        topBarRt.pivot = new Vector2(0.5f, 1);
        topBarRt.anchoredPosition = new Vector2(0, -20);
        topBarRt.sizeDelta = new Vector2(-40, 50);
        uiManager.topBarText = topBarText;
        
        // Buttons Container
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
        
        GenerateScriptableObjects();
        
        Debug.Log("Defense Scene setup complete!");
    }
    
    private static void GenerateScriptableObjects()
    {
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources/Buildings"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy/Resources", "Buildings");
        if (!AssetDatabase.IsValidFolder("Assets/0_Adventure/Deploy/Resources/Monsters"))
            AssetDatabase.CreateFolder("Assets/0_Adventure/Deploy/Resources", "Monsters");
        
        // Mines
        CreateBuildingData("StoneMine", BuildingType.Mine, 100, 1, 0.1f, ResourceType.None, ResourceType.None, 0, ResourceType.Stone, 50);
        CreateBuildingData("IronMine", BuildingType.Mine, 100, 1, 0.1f, ResourceType.None, ResourceType.None, 0, ResourceType.Iron, 50);
        
        // Entrance
        CreateBuildingData("Entrance", BuildingType.Entrance, 0, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0);

        // Road
        CreateBuildingData("Road", BuildingType.Road, 100, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0);
        
        // Factories
        CreateBuildingData("StoneFactory", BuildingType.Factory, 100, 1, 1f, ResourceType.Stone, ResourceType.None, 5, ResourceType.RoundStone, 5);
        CreateBuildingData("ArrowFactory", BuildingType.Factory, 150, 1, 1f, ResourceType.Iron, ResourceType.None, 5, ResourceType.Arrow, 5);
        CreateBuildingData("AmmoFactory", BuildingType.Factory, 200, 1, 1f, ResourceType.Arrow, ResourceType.RoundStone, 5, ResourceType.GunAmmo, 5);
        
        // Buffs
        CreateBuildingData("FactoryBuff", BuildingType.FactorySpeedBuff, 100, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, buffAmt: 1f, buffRng: 1);
        CreateBuildingData("TowerBuff", BuildingType.TowerAttackBuff, 200, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, buffAmt: 0.5f, buffRng: 1);

        // Towers
        CreateBuildingData("Slingshot", BuildingType.Tower, 150, 1, 0, ResourceType.None, ResourceType.None, 0, ResourceType.None, 0, atk: 50f, atkSpd: 0.7f, atkRange: 6, maxAmmo: 3, reqAmmo: ResourceType.Stone);
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
        
        AssetDatabase.SaveAssets();
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
