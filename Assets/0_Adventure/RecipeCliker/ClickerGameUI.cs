using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 확률 설정 (꽃, 가공된 목재, 수액)")]
    [Range(0f, 100f)] public float flowerChance = 3.0f;
    [Range(0f, 100f)] public float woodChance = 5.0f;
    [Range(0f, 100f)] public float sapChance = 15.0f;
    [Range(0f, 100f)] public float forestRecChance = 2.0f;

    [Header("⛰️ 광산 확률 설정 (금광석, 특수 합금, 은광석)")]
    [Range(0f, 100f)] public float goldChance = 2.0f;
    [Range(0f, 100f)] public float alloyChance = 2.0f;
    [Range(0f, 100f)] public float silverChance = 20.0f;
    [Range(0f, 100f)] public float mineRecChance = 1.0f;

    [Header("💰 자원 판매 가격")]
    public int logPrice = 10; public int sapPrice = 50; public int flowerPrice = 150; public int woodPrice = 200;
    public int ironPrice = 15; public int silverPrice = 80; public int goldPrice = 250; public int alloyPrice = 300;
    public int artifactPrice = 5000;

    [Header("🛒 상점 비용 설정")]
    public float sellBonusPerLevel = 0.05f;
    public int baseSellBonusCost = 1000;
    public int stonePickCost = 2000; public int ironPickCost = 5000; public int shearsCost = 4000;
    public int woodRecCost = 1000; public int alloyRecCost = 2500; public int artifactRecCost = 10000;
    public int endingDeedCost = 500000;

    private ClickerState s = new ClickerState(new Inventory(0, 0, 0, 0, 0, 0, 0, 0, 0), 0, new UpgradeState(0));
    private int Boosted(int baseP) => Mathf.RoundToInt(baseP * (1f + s.Upgrades.SellBonus * sellBonusPerLevel));

    private string notificationMsg = "";
    private float notificationEndTime = 0f;

    private void ShowNotification(string msg)
    {
        notificationMsg = msg;
        notificationEndTime = Time.time + 2.0f;
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 16; GUI.skin.button.fontSize = 16;
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));

        GUILayout.BeginHorizontal();
        GUILayout.Label("🛠️ 직관적 조합 클리커 - 도구 & 땅문서 완전판", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        // 🟢 [좌측 구역] 숲과 광산을 묶는 컨테이너
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.66f - 30));
        GUILayout.BeginHorizontal();

        // --- 1. 🌲 숲 구역 ---
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 20));
        DrawInventoryBox("🌲 숲 채집물",
            ("통나무", s.Inv.Log, Boosted(logPrice), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = i.Log - 1 }, 1) : (i, 0), Boosted(logPrice)), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = 0 }, i.Log) : (i, 0), Boosted(logPrice))),
            ("수액", s.Inv.Sap, Boosted(sapPrice), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = i.Sap - 1 }, 1) : (i, 0), Boosted(sapPrice)), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = 0 }, i.Sap) : (i, 0), Boosted(sapPrice))),
            ("꽃", s.Inv.Flower, Boosted(flowerPrice), () => s = GameLogic.Sell(s, i => i.Flower > 0 ? (i with { Flower = i.Flower - 1 }, 1) : (i, 0), Boosted(flowerPrice)), () => s = GameLogic.Sell(s, i => i.Flower > 0 ? (i with { Flower = 0 }, i.Flower) : (i, 0), Boosted(flowerPrice))),
            ("가공 목재", s.Inv.Wood, Boosted(woodPrice), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = i.Wood - 1 }, 1) : (i, 0), Boosted(woodPrice)), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = 0 }, i.Wood) : (i, 0), Boosted(woodPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🌲 숲 행동");
        float fLogCh = 100f - woodChance - sapChance - (s.Upgrades.HasShears ? flowerChance : 0f);
        string fDrops = $"💎 [드롭] 통나무({fLogCh:F1}%) / 수액({sapChance}%) / 목재({woodChance}%)" + (s.Upgrades.HasShears ? $" / 꽃({flowerChance}%)" : "");
        GUILayout.Label(fDrops, new GUIStyle(GUI.skin.label) { fontSize = 13 });

        string wRecStr = s.Upgrades.HasWoodRec ? "✅" : "❌";
        string pRecStr = s.Upgrades.HasStonePickRec ? "✅" : "❌";
        GUILayout.Label($"📜 [레시피] 목재({wRecStr}) / 돌곡괭이({pRecStr})", new GUIStyle(GUI.skin.label) { fontSize = 13 });
        GUILayout.Space(5);

        if (GUILayout.Button($"⛏️ 숲 채집하기", GUILayout.Height(60)))
        {
            bool hWood = s.Upgrades.HasWoodRec; bool hSPick = s.Upgrades.HasStonePickRec;
            s = GameLogic.GatherForest(s, Random.Range(0f, 100f), woodChance, sapChance, flowerChance);
            s = GameLogic.RollForestRecipes(s, Random.Range(0f, 100f), forestRecChance, Random.Range(0f, 100f), forestRecChance);
            if (!hWood && s.Upgrades.HasWoodRec) ShowNotification("📜 [가공된 목재 레시피] 발견!");
            if (!hSPick && s.Upgrades.HasStonePickRec) ShowNotification("📜 [돌 곡괭이 레시피] 발견!");
        }
        GUILayout.Space(5);
        if (s.Upgrades.HasWoodRec) DrawCraftButton("🔨 가공된 목재 [통나무(10) + 수액(3)]", GameLogic.CanCraftWood(s), () => s = GameLogic.CraftWood(s));
        if (s.Upgrades.HasStonePickRec && !s.Upgrades.HasStonePick) DrawCraftButton("⛏️ 돌 곡괭이 [통나무(20) + 목재(2)]", GameLogic.CanCraftStonePick(s), () => { s = GameLogic.CraftStonePick(s); ShowNotification("⛏️ 광산 해금!"); });
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 2. ⛰️ 광산 구역 ---
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 20));
        if (s.Upgrades.HasStonePick)
        {
            DrawInventoryBox("⛰️ 광산 채집물",
                ("철광석", s.Inv.Iron, Boosted(ironPrice), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = i.Iron - 1 }, 1) : (i, 0), Boosted(ironPrice)), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = 0 }, i.Iron) : (i, 0), Boosted(ironPrice))),
                ("은광석", s.Inv.Silver, Boosted(silverPrice), () => s = GameLogic.Sell(s, i => i.Silver > 0 ? (i with { Silver = i.Silver - 1 }, 1) : (i, 0), Boosted(silverPrice)), () => s = GameLogic.Sell(s, i => i.Silver > 0 ? (i with { Silver = 0 }, i.Silver) : (i, 0), Boosted(silverPrice))),
                ("금광석", s.Inv.Gold, Boosted(goldPrice), () => s = GameLogic.Sell(s, i => i.Gold > 0 ? (i with { Gold = i.Gold - 1 }, 1) : (i, 0), Boosted(goldPrice)), () => s = GameLogic.Sell(s, i => i.Gold > 0 ? (i with { Gold = 0 }, i.Gold) : (i, 0), Boosted(goldPrice))),
                ("특수 합금", s.Inv.Alloy, Boosted(alloyPrice), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = i.Alloy - 1 }, 1) : (i, 0), Boosted(alloyPrice)), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = 0 }, i.Alloy) : (i, 0), Boosted(alloyPrice)))
            );
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 광산 행동");
            float mIronCh = 100f - alloyChance - silverChance - (s.Upgrades.HasIronPick ? goldChance : 0f);
            string mDrops = $"💎 [드롭] 철({mIronCh:F1}%) / 은({silverChance}%) / 합금({alloyChance}%)" + (s.Upgrades.HasIronPick ? $" / 금({goldChance}%)" : "");
            GUILayout.Label(mDrops, new GUIStyle(GUI.skin.label) { fontSize = 13 });

            // ✨ 컴파일 에러 해결
            string aRecStr = s.Upgrades.HasAlloyRec ? "✅" : "❌";
            string iRecStr = s.Upgrades.HasIronPickRec ? "✅" : "❌";
            string sRecStr = s.Upgrades.HasShearsRec ? "✅" : "❌";
            GUILayout.Label($"📜 [레시피] 합금({aRecStr})/철곡괭이({iRecStr})/가위({sRecStr})", new GUIStyle(GUI.skin.label) { fontSize = 13 });
            GUILayout.Space(5);

            if (GUILayout.Button($"⛏️ 광산 채집하기", GUILayout.Height(60)))
            {
                bool hAlloy = s.Upgrades.HasAlloyRec; bool hIPick = s.Upgrades.HasIronPickRec; bool hShears = s.Upgrades.HasShearsRec;
                s = GameLogic.GatherMine(s, Random.Range(0f, 100f), alloyChance, silverChance, goldChance);
                s = GameLogic.RollMineRecipes(s, Random.Range(0f, 100f), mineRecChance, Random.Range(0f, 100f), mineRecChance, Random.Range(0f, 100f), mineRecChance);
                if (!hAlloy && s.Upgrades.HasAlloyRec) ShowNotification("📜 [특수 합금 레시피] 발견!");
                if (!hIPick && s.Upgrades.HasIronPickRec) ShowNotification("📜 [철 곡괭이 레시피] 발견!");
                if (!hShears && s.Upgrades.HasShearsRec) ShowNotification("📜 [원예 가위 레시피] 발견!");
            }
            GUILayout.Space(5);
            if (s.Upgrades.HasAlloyRec) DrawCraftButton("🔨 특수 합금 [철광석(10) + 은광석(3)]", GameLogic.CanCraftAlloy(s), () => s = GameLogic.CraftAlloy(s));
            if (s.Upgrades.HasIronPickRec && !s.Upgrades.HasIronPick) DrawCraftButton("⛏️ 철 곡괭이 [철광석(20) + 합금(2)]", GameLogic.CanCraftIronPick(s), () => { s = GameLogic.CraftIronPick(s); ShowNotification("⛏️ 금광석 해금!"); });
            if (s.Upgrades.HasShearsRec && !s.Upgrades.HasShears) DrawCraftButton("✂️ 원예 가위 [은광석(20) + 합금(1)]", GameLogic.CanCraftShears(s), () => { s = GameLogic.CraftShears(s); ShowNotification("✂️ 꽃 해금!"); });
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 미개척 구역");
            GUILayout.FlexibleSpace();
            GUILayout.Label("🔒 돌 곡괭이가 필요합니다.", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        if (s.Upgrades.HasArtifactRec)
        {
            DrawCraftButton("✨ 궁극의 공예품 조합 ✨\n[꽃(1) + 금광석(1)]", GameLogic.CanCraftArtifact(s), () => s = GameLogic.CraftArtifact(s), 70);
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // 🔵 [우측 구역] 상점 및 결과물
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 30));
        DrawInventoryBox("✨ 특수 자원 보관함",
            ("공예품", s.Inv.Artifact, Boosted(artifactPrice), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = i.Artifact - 1 }, 1) : (i, 0), Boosted(artifactPrice)), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = 0 }, i.Artifact) : (i, 0), Boosted(artifactPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점 (도구 및 레시피 직거래)");
        int upgCost = baseSellBonusCost * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"판매 효율 +{sellBonusPerLevel * 100f:F0}% (현재 +{s.Upgrades.SellBonus * sellBonusPerLevel * 100f:F0}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }));
        GUILayout.Space(10);

        if (!s.Upgrades.HasStonePick) DrawShopRow($"⛏️ 돌 곡괭이 (광산 해금)", stonePickCost, () => {
            s = GameLogic.BuyUpgrade(s, stonePickCost, u => u with { HasStonePick = true });
            if (s.Upgrades.HasStonePick) ShowNotification("⛏️ 광산 해금!");
        });
        if (!s.Upgrades.HasIronPick) DrawShopRow($"⛏️ 철 곡괭이 (금광석 해금)", ironPickCost, () => {
            s = GameLogic.BuyUpgrade(s, ironPickCost, u => u with { HasIronPick = true });
            if (s.Upgrades.HasIronPick) ShowNotification("⛏️ 금광석 해금!");
        });
        if (!s.Upgrades.HasShears) DrawShopRow($"✂️ 원예 가위 (꽃 해금)", shearsCost, () => {
            s = GameLogic.BuyUpgrade(s, shearsCost, u => u with { HasShears = true });
            if (s.Upgrades.HasShears) ShowNotification("✂️ 꽃 해금!");
        });

        if (!s.Upgrades.HasWoodRec) DrawShopRow($"📜 가공된 목재 레시피", woodRecCost, () => s = GameLogic.BuyUpgrade(s, woodRecCost, u => u with { HasWoodRec = true }));
        if (!s.Upgrades.HasAlloyRec) DrawShopRow($"📜 특수 합금 레시피", alloyRecCost, () => s = GameLogic.BuyUpgrade(s, alloyRecCost, u => u with { HasAlloyRec = true }));

        if (!s.Upgrades.HasArtifactRec) DrawShopRow($"✨ 공예품 레시피", artifactRecCost, () => {
            s = GameLogic.BuyUpgrade(s, artifactRecCost, u => u with { HasArtifactRec = true });
            if (s.Upgrades.HasArtifactRec) ShowNotification("✨ 공예품 레시피 획득!");
        });

        // ✨ 영주의 땅문서 완벽 부활
        if (!s.Upgrades.HasEndingDeed) DrawShopRow($"👑 영주의 땅문서", endingDeedCost, () => {
            s = GameLogic.BuyUpgrade(s, endingDeedCost, u => u with { HasEndingDeed = true });
            if (s.Upgrades.HasEndingDeed) ShowNotification("🎉 영주의 땅문서를 구매했습니다!");
        });

        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ==================== 🌟 알림 UI ====================
        if (Time.time < notificationEndTime)
        {
            GUIStyle notifStyle = new GUIStyle(GUI.skin.box);
            notifStyle.fontSize = 26;
            notifStyle.normal.textColor = Color.yellow;
            notifStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Box(new Rect(Screen.width / 2f - 300, Screen.height / 2f - 50, 600, 100), notificationMsg, notifStyle);
        }
    }

    private void DrawInventoryBox(string title, params (string name, int amount, int price, System.Action onSell, System.Action onSellAll)[] items)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(5);
        foreach (var item in items)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.name, GUILayout.Width(90));
            GUILayout.Label($"{item.amount}개", GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            GUI.enabled = item.amount > 0;
            if (GUILayout.Button($"1개(+{item.price:N0})", GUILayout.Width(105))) item.onSell?.Invoke();
            if (GUILayout.Button($"일괄(+{item.amount * item.price:N0})", GUILayout.Width(120))) item.onSellAll?.Invoke();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        GUILayout.EndVertical();
    }

    private void DrawCraftButton(string label, bool canCraft, System.Action onCraft, int height = 50)
    {
        GUI.enabled = canCraft;
        if (GUILayout.Button(label, GUILayout.Height(height))) onCraft?.Invoke();
        GUI.enabled = true;
        GUILayout.Space(5);
    }

    private void DrawShopRow(string label, int cost, System.Action onBuy)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        GUILayout.FlexibleSpace();
        GUI.enabled = s.Money >= cost;
        if (GUILayout.Button($"💰 {cost:N0}G", GUILayout.Width(130))) onBuy?.Invoke();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }
}