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
        
        // Summon Button 1
        var summonBtn1Go = new GameObject("SummonButton1");
        summonBtn1Go.transform.SetParent(canvasGo.transform, false);
        summonBtn1Go.AddComponent<Image>().color = Color.white;
        var summonBtn1 = summonBtn1Go.AddComponent<Button>();
        var summon1Rt = summonBtn1Go.GetComponent<RectTransform>();
        summon1Rt.anchorMin = new Vector2(0.3f, 0);
        summon1Rt.anchorMax = new Vector2(0.3f, 0);
        summon1Rt.pivot = new Vector2(0.5f, 0);
        summon1Rt.anchoredPosition = new Vector2(0, 20);
        summon1Rt.sizeDelta = new Vector2(200, 80);
        var summonText1Go = new GameObject("Text");
        summonText1Go.transform.SetParent(summonBtn1Go.transform, false);
        var summonText1 = summonText1Go.AddComponent<TextMeshProUGUI>();
        summonText1.text = "Summon 1";
        summonText1.color = Color.black;
        summonText1.alignment = TextAlignmentOptions.Center;
        summonText1.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
        uiManager.summonButton1 = summonBtn1;
        
        // Summon Button 2
        var summonBtn2Go = new GameObject("SummonButton2");
        summonBtn2Go.transform.SetParent(canvasGo.transform, false);
        summonBtn2Go.AddComponent<Image>().color = Color.white;
        var summonBtn2 = summonBtn2Go.AddComponent<Button>();
        var summon2Rt = summonBtn2Go.GetComponent<RectTransform>();
        summon2Rt.anchorMin = new Vector2(0.55f, 0);
        summon2Rt.anchorMax = new Vector2(0.55f, 0);
        summon2Rt.pivot = new Vector2(0.5f, 0);
        summon2Rt.anchoredPosition = new Vector2(0, 20);
        summon2Rt.sizeDelta = new Vector2(200, 80);
        var summonText2Go = new GameObject("Text");
        summonText2Go.transform.SetParent(summonBtn2Go.transform, false);
        var summonText2 = summonText2Go.AddComponent<TextMeshProUGUI>();
        summonText2.text = "Summon 2";
        summonText2.color = Color.black;
        summonText2.alignment = TextAlignmentOptions.Center;
        summonText2.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
        uiManager.summonButton2 = summonBtn2;
        
        // Build Road Button
        var roadBtnGo = new GameObject("BuildRoadButton");
        roadBtnGo.transform.SetParent(canvasGo.transform, false);
        roadBtnGo.AddComponent<Image>().color = Color.gray;
        var roadBtn = roadBtnGo.AddComponent<Button>();
        var roadRt = roadBtnGo.GetComponent<RectTransform>();
        roadRt.anchorMin = new Vector2(0.85f, 0);
        roadRt.anchorMax = new Vector2(0.85f, 0);
        roadRt.pivot = new Vector2(0.5f, 0);
        roadRt.anchoredPosition = new Vector2(0, 20);
        roadRt.sizeDelta = new Vector2(200, 80);
        var roadTextGo = new GameObject("Text");
        roadTextGo.transform.SetParent(roadBtnGo.transform, false);
        var roadText = roadTextGo.AddComponent<TextMeshProUGUI>();
        roadText.text = "Build Road\n(Click Map)";
        roadText.color = Color.white;
        roadText.alignment = TextAlignmentOptions.Center;
        roadText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
        uiManager.buildRoadButton = roadBtn;
        
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
        
        CreateBuildingData("BasicFactory", BuildingType.Factory, 2f, AmmoType.Normal, 1);
        CreateBuildingData("FireFactory", BuildingType.Factory, 3f, AmmoType.Fire, 1);
        CreateBuildingData("Road", BuildingType.Road, 0f, AmmoType.Normal, 1);
        CreateBuildingData("BasicTower", BuildingType.Tower, 0f, AmmoType.Normal, 0, 10f, 1f, 3f, 10, 0);
        CreateBuildingData("FastLongTower", BuildingType.Tower, 0f, AmmoType.Normal, 0, 5f, 0.3f, 7f, 15, 0);
        CreateBuildingData("SpeedBuff", BuildingType.FactorySpeedBuff, 0f, AmmoType.Normal, 2, 0, 0, 0, 0, 0.5f);
        CreateBuildingData("AttackBuff", BuildingType.TowerAttackBuff, 0f, AmmoType.Normal, 2, 0, 0, 0, 0, 5f);
        
        var monsterData = ScriptableObject.CreateInstance<MonsterDataSO>();
        monsterData.maxHp = 50f;
        monsterData.speed = 1f; // Slowed down from 2f to 1f
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
