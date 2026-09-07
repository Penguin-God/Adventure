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
        
        CreateBuildingData("Factory", BuildingType.Factory, 1f, AmmoType.Normal, 1, 0, 0, 0, 0, 0, 1, 200);
        CreateBuildingData("Road", BuildingType.Road, 0f, AmmoType.Normal, 1, 0, 0, 0, 0, 0, 1, 100);
        CreateBuildingData("Cannon", BuildingType.Tower, 0f, AmmoType.Normal, 0, 50f, 0.7f, 2f, 3, 0, 1, 150);
        CreateBuildingData("Archer", BuildingType.Tower, 0f, AmmoType.Normal, 0, 35f, 1.2f, 3f, 5, 0, 1, 200);
        CreateBuildingData("FactoryBuff", BuildingType.FactorySpeedBuff, 0f, AmmoType.Normal, 0, 0, 0, 0, 0, 1f, 1, 100);
        CreateBuildingData("TowerBuff", BuildingType.TowerAttackBuff, 0f, AmmoType.Normal, 0, 0, 0, 0, 0, 0.5f, 1, 200);
        
        var monsterData = ScriptableObject.CreateInstance<MonsterDataSO>();
        monsterData.maxHp = 100f; // changed base HP from 50 to 200 based on settings
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
        settings.monsterBaseHp = 100f;
        settings.monsterHpIncreaseStep = 10;
        settings.monsterHpIncreasePercent = 0.2f;
        EditorUtility.SetDirty(settings);
        
        AssetDatabase.SaveAssets();
    }
    
    private static void CreateBuildingData(string name, BuildingType type, float ammoProdSec, AmmoType ammoType, int connRange, float atk, float atkSpd, float atkRange, int maxAmmo, float buffAmt, int buffRng, int cost)
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
        b.ammoProductionTime = ammoProdSec;
        b.ammoType = ammoType;
        b.connectionRange = connRange;
        b.attackDamage = atk;
        b.attackSpeed = atkSpd;
        b.attackRange = atkRange;
        b.maxAmmo = maxAmmo;
        b.buffAmount = buffAmt;
        b.buffRange = buffRng;
        b.cost = cost;
        
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/0_Adventure/Deploy/Resources/Sprites/{name}.png");
        if (sprite != null) b.sprite = sprite;
        
        EditorUtility.SetDirty(b);
    }
}
