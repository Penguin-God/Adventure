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
        managersParent.AddComponent<DeckManager>();
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
        
        // Summon Button
        var summonBtnGo = new GameObject("SummonButton");
        summonBtnGo.transform.SetParent(canvasGo.transform, false);
        summonBtnGo.AddComponent<Image>().color = Color.white;
        var summonBtn = summonBtnGo.AddComponent<Button>();
        var summonRt = summonBtnGo.GetComponent<RectTransform>();
        summonRt.anchorMin = new Vector2(0.5f, 0);
        summonRt.anchorMax = new Vector2(0.5f, 0);
        summonRt.pivot = new Vector2(0.5f, 0);
        summonRt.anchoredPosition = new Vector2(0, 20);
        summonRt.sizeDelta = new Vector2(200, 80);
        var summonTextGo = new GameObject("Text");
        summonTextGo.transform.SetParent(summonBtnGo.transform, false);
        var summonText = summonTextGo.AddComponent<TextMeshProUGUI>();
        summonText.text = "Summon (100G)";
        summonText.color = Color.black;
        summonText.alignment = TextAlignmentOptions.Center;
        summonText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
        uiManager.summonButton = summonBtn;
        
        // Build Road Button
        var roadBtnGo = new GameObject("BuildRoadButton");
        roadBtnGo.transform.SetParent(canvasGo.transform, false);
        roadBtnGo.AddComponent<Image>().color = Color.gray;
        var roadBtn = roadBtnGo.AddComponent<Button>();
        var roadRt = roadBtnGo.GetComponent<RectTransform>();
        roadRt.anchorMin = new Vector2(0.8f, 0);
        roadRt.anchorMax = new Vector2(0.8f, 0);
        roadRt.pivot = new Vector2(0.5f, 0);
        roadRt.anchoredPosition = new Vector2(0, 20);
        roadRt.sizeDelta = new Vector2(200, 80);
        var roadTextGo = new GameObject("Text");
        roadTextGo.transform.SetParent(roadBtnGo.transform, false);
        var roadText = roadTextGo.AddComponent<TextMeshProUGUI>();
        roadText.text = "Build Road (50G)";
        roadText.color = Color.white;
        roadText.alignment = TextAlignmentOptions.Center;
        roadText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
        uiManager.buildRoadButton = roadBtn;
        
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
        
        CreateBuildingData("BasicFactory", BuildingType.Factory, 2f, AmmoType.Normal, 2);
        CreateBuildingData("FireFactory", BuildingType.Factory, 3f, AmmoType.Fire, 2);
        CreateBuildingData("Road", BuildingType.Road, 0f, AmmoType.Normal, 1);
        CreateBuildingData("BasicTower", BuildingType.Tower, 0f, AmmoType.Normal, 0, 10f, 1f, 3f, 10);
        CreateBuildingData("SpeedBuff", BuildingType.FactorySpeedBuff, 0f, AmmoType.Normal, 2, 0, 0, 0, 0, 0.5f);
        CreateBuildingData("AttackBuff", BuildingType.TowerAttackBuff, 0f, AmmoType.Normal, 2, 0, 0, 0, 0, 5f);
        
        var monsterData = ScriptableObject.CreateInstance<MonsterDataSO>();
        monsterData.maxHp = 50f;
        monsterData.speed = 2f;
        monsterData.rewardGold = 20;
        AssetDatabase.CreateAsset(monsterData, "Assets/0_Adventure/Deploy/Resources/Monsters/BasicMonster.asset");
        
        AssetDatabase.SaveAssets();
    }
    
    private static void CreateBuildingData(string name, BuildingType type, float prodTime, AmmoType ammoType, int range, float atk = 0, float atkSpd = 0, float atkRange = 0, int maxAmmo = 0, float buff = 0)
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
        b.ammoProductionTime = prodTime;
        b.producedAmmoType = ammoType;
        b.connectionRange = range;
        b.buffRange = range;
        b.attackDamage = atk;
        b.attackSpeed = atkSpd;
        b.attackRange = atkRange;
        b.maxAmmo = maxAmmo;
        b.ammoType = ammoType;
        b.buffAmount = buff;
        EditorUtility.SetDirty(b);
    }
}
