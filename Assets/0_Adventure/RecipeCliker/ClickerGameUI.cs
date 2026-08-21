using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 기본 자원 확률 (꽃, 수액)")]
    [Range(0f, 100f)] public float flowerChance = 3.0f;
    [Range(0f, 100f)] public float sapChance = 15.0f;

    // ✨ 레시피별 획득 확률을 각각 분리했습니다!
    [Header("🌲 숲 레시피 드롭 확률")]
    [Range(0f, 100f)] public float woodRecChance = 2.0f;
    [Range(0f, 100f)] public float woodPickRecChance = 1.0f;

    [Header("⛰️ 광산 기본 자원 확률 (금광석, 철광석)")]
    [Range(0f, 100f)] public float goldChance = 2.0f;
    [Range(0f, 100f)] public float ironChance = 20.0f;

    // ✨ 광산 역시 레시피별 획득 확률을 완벽히 분리했습니다!
    [Header("⛰️ 광산 레시피 드롭 확률")]
    [Range(0f, 100f)] public float alloyRecChance = 2.0f;
    [Range(0f, 100f)] public float ironPickRecChance = 1.0f;
    [Range(0f, 100f)] public float shearsRecChance = 1.0f;

    [Header("💰 자원 판매 가격")]
    public int logPrice = 10; public int sapPrice = 50; public int flowerPrice = 150; public int woodPrice = 200;
    public int stonePrice = 10; public int ironPrice = 80; public int goldPrice = 250; public int alloyPrice = 300;
    public int artifactPrice = 5000;

    [Header("🛒 상점 비용 설정 (오직 레시피만)")]
    public float sellBonusPerLevel = 0.05f;
    public int baseSellBonusCost = 1000;
    public int woodRecCost = 1000; public int alloyRecCost = 2500; public int artifactRecCost = 10000;
    public int woodPickRecCost = 2000; public int ironPickRecCost = 5000; public int shearsRecCost = 4000;

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
        GUILayout.Label("🛠️ 직관적 조합 클리커 - 개별 레시피 확률 분리", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        // 🟢 [좌측 구역] 숲과 광산
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
        float fLogCh = 100f - sapChance - (s.Upgrades.HasShears ? flowerChance : 0f);
        string fDrops = $"💎 [드롭] 통나무({fLogCh:F1}%) / 수액({sapChance}%)" + (s.Upgrades.HasShears ? $" / 꽃({flowerChance}%)" : "");
        GUILayout.Label(fDrops, new GUIStyle(GUI.skin.label) { fontSize = 13 });

        // ✨ 각각 분리된 확률을 화면에 정확히 띄워줍니다!
        string wRecStr = s.Upgrades.HasWoodRec ? "✅" : "❌";
        string pRecStr = s.Upgrades.HasWoodPickRec ? "✅" : "❌";
        GUILayout.Label($"📜 [레시피] 목재({woodRecChance}%) {wRecStr} / 나무곡괭이({woodPickRecChance}%) {pRecStr}", new GUIStyle(GUI.skin.label) { fontSize = 13 });
        GUILayout.Space(5);

        if (GUILayout.Button($"⛏️ 숲 채집하기", GUILayout.Height(60)))
        {
            bool hWood = s.Upgrades.HasWoodRec; bool hWPick = s.Upgrades.HasWoodPickRec;
            s = GameLogic.GatherForest(s, Random.Range(0f, 100f), sapChance, flowerChance);

            // ✨ 숲 레시피 주사위를 굴릴 때, 각각 분리된 확률 변수를 던져줍니다!
            s = GameLogic.RollForestRecipes(s, Random.Range(0f, 100f), woodRecChance, Random.Range(0f, 100f), woodPickRecChance);

            if (!hWood && s.Upgrades.HasWoodRec) ShowNotification("📜 [가공된 목재 레시피] 발견!");
            if (!hWPick && s.Upgrades.HasWoodPickRec) ShowNotification("📜 [나무 곡괭이 레시피] 발견!");
        }
        GUILayout.Space(5);

        if (s.Upgrades.HasWoodRec) DrawCraftButton("🔨 가공된 목재 [통나무(10) + 수액(3)]", GameLogic.CanCraftWood(s), () => s = GameLogic.CraftWood(s));
        if (s.Upgrades.HasWoodPickRec && !s.Upgrades.HasWoodPick) DrawCraftButton("⛏️ 나무 곡괭이 제작 [목재(10)]", GameLogic.CanCraftWoodPick(s), () => { s = GameLogic.CraftWoodPick(s); ShowNotification("⛏️ 광산 해금!"); });
        if (s.Upgrades.HasShearsRec && !s.Upgrades.HasShears) DrawCraftButton("✂️ 원예 가위 제작 [철광석(10) + 통나무(30)]", GameLogic.CanCraftShears(s), () => { s = GameLogic.CraftShears(s); ShowNotification("✂️ 꽃 해금!"); });

        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 2. ⛰️ 광산 구역 ---
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 20));
        if (s.Upgrades.HasWoodPick)
        {
            DrawInventoryBox("⛰️ 광산 채집물",
                ("돌", s.Inv.Stone, Boosted(stonePrice), () => s = GameLogic.Sell(s, i => i.Stone > 0 ? (i with { Stone = i.Stone - 1 }, 1) : (i, 0), Boosted(stonePrice)), () => s = GameLogic.Sell(s, i => i.Stone > 0 ? (i with { Stone = 0 }, i.Stone) : (i, 0), Boosted(stonePrice))),
                ("철광석", s.Inv.Iron, Boosted(ironPrice), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = i.Iron - 1 }, 1) : (i, 0), Boosted(ironPrice)), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = 0 }, i.Iron) : (i, 0), Boosted(ironPrice))),
                ("금광석", s.Inv.Gold, Boosted(goldPrice), () => s = GameLogic.Sell(s, i => i.Gold > 0 ? (i with { Gold = i.Gold - 1 }, 1) : (i, 0), Boosted(goldPrice)), () => s = GameLogic.Sell(s, i => i.Gold > 0 ? (i with { Gold = 0 }, i.Gold) : (i, 0), Boosted(goldPrice))),
                ("특수 합금", s.Inv.Alloy, Boosted(alloyPrice), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = i.Alloy - 1 }, 1) : (i, 0), Boosted(alloyPrice)), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = 0 }, i.Alloy) : (i, 0), Boosted(alloyPrice)))
            );
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 광산 행동");
            float mStoneCh = 100f - ironChance - (s.Upgrades.HasIronPick ? goldChance : 0f);
            string mDrops = $"💎 [드롭] 돌({mStoneCh:F1}%) / 철광석({ironChance}%)" + (s.Upgrades.HasIronPick ? $" / 금({goldChance}%)" : "");
            GUILayout.Label(mDrops, new GUIStyle(GUI.skin.label) { fontSize = 13 });

            // ✨ 광산 레시피 확률 분리 출력
            string aRecStr = s.Upgrades.HasAlloyRec ? "✅" : "❌";
            string iRecStr = s.Upgrades.HasIronPickRec ? "✅" : "❌";
            string sRecStr = s.Upgrades.HasShearsRec ? "✅" : "❌";
            GUILayout.Label($"📜 [레시피] 합금({alloyRecChance}%) {aRecStr} / 철곡괭이({ironPickRecChance}%) {iRecStr} / 가위({shearsRecChance}%) {sRecStr}", new GUIStyle(GUI.skin.label) { fontSize = 13 });
            GUILayout.Space(5);

            if (GUILayout.Button($"⛏️ 광산 채집하기", GUILayout.Height(60)))
            {
                bool hAlloy = s.Upgrades.HasAlloyRec; bool hIPick = s.Upgrades.HasIronPickRec; bool hShears = s.Upgrades.HasShearsRec;
                s = GameLogic.GatherMine(s, Random.Range(0f, 100f), ironChance, goldChance);

                // ✨ 광산 레시피 주사위를 굴릴 때도, 각각 분리된 확률 변수를 던져줍니다!
                s = GameLogic.RollMineRecipes(s, Random.Range(0f, 100f), alloyRecChance, Random.Range(0f, 100f), ironPickRecChance, Random.Range(0f, 100f), shearsRecChance);

                if (!hAlloy && s.Upgrades.HasAlloyRec) ShowNotification("📜 [특수 합금 레시피] 발견!");
                if (!hIPick && s.Upgrades.HasIronPickRec) ShowNotification("📜 [철 곡괭이 레시피] 발견!");
                if (!hShears && s.Upgrades.HasShearsRec) ShowNotification("📜 [원예 가위 레시피] 발견!");
            }
            GUILayout.Space(5);
            if (s.Upgrades.HasAlloyRec) DrawCraftButton("🔨 특수 합금 [돌(10) + 철광석(3)]", GameLogic.CanCraftAlloy(s), () => s = GameLogic.CraftAlloy(s));
            if (s.Upgrades.HasIronPickRec && !s.Upgrades.HasIronPick) DrawCraftButton("⛏️ 철 곡괭이 제작 [나무곡괭이 + 철(30)]", GameLogic.CanCraftIronPick(s), () => { s = GameLogic.CraftIronPick(s); ShowNotification("⛏️ 금광석 해금!"); });
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 미개척 구역");
            GUILayout.FlexibleSpace();
            GUILayout.Label("🔒 나무 곡괭이가 필요합니다.", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
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

        GUILayout.BeginVertical("box");
        GUILayout.Label("🎒 보유 장비", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        string toolList = "";
        if (s.Upgrades.HasWoodPick) toolList += "⛏️ 나무 곡괭이  ";
        if (s.Upgrades.HasIronPick) toolList += "⛏️ 철 곡괭이  ";
        if (s.Upgrades.HasShears) toolList += "✂️ 원예 가위";
        if (toolList == "") toolList = "맨손";
        GUILayout.Label(toolList, new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.EndVertical();
        GUILayout.Space(10);

        DrawInventoryBox("✨ 특수 자원 보관함",
            ("공예품", s.Inv.Artifact, Boosted(artifactPrice), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = i.Artifact - 1 }, 1) : (i, 0), Boosted(artifactPrice)), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = 0 }, i.Artifact) : (i, 0), Boosted(artifactPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점 (오직 레시피만 취급)");
        int upgCost = baseSellBonusCost * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"판매 효율 +{sellBonusPerLevel * 100f:F0}% (현재 +{s.Upgrades.SellBonus * sellBonusPerLevel * 100f:F0}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }), true);
        GUILayout.Space(10);

        if (!s.Upgrades.HasWoodRec) DrawShopRow($"📜 가공된 목재 레시피", woodRecCost, () => s = GameLogic.BuyUpgrade(s, woodRecCost, u => u with { HasWoodRec = true }));
        if (!s.Upgrades.HasWoodPickRec) DrawShopRow($"📜 나무 곡괭이 레시피", woodPickRecCost, () => s = GameLogic.BuyUpgrade(s, woodPickRecCost, u => u with { HasWoodPickRec = true }));
        if (!s.Upgrades.HasAlloyRec) DrawShopRow($"📜 특수 합금 레시피", alloyRecCost, () => s = GameLogic.BuyUpgrade(s, alloyRecCost, u => u with { HasAlloyRec = true }));
        if (!s.Upgrades.HasIronPickRec) DrawShopRow($"📜 철 곡괭이 레시피", ironPickRecCost, () => s = GameLogic.BuyUpgrade(s, ironPickRecCost, u => u with { HasIronPickRec = true }));
        if (!s.Upgrades.HasShearsRec) DrawShopRow($"📜 원예 가위 레시피", shearsRecCost, () => s = GameLogic.BuyUpgrade(s, shearsRecCost, u => u with { HasShearsRec = true }));

        if (!s.Upgrades.HasArtifactRec) DrawShopRow($"✨ 공예품 레시피", artifactRecCost, () => {
            s = GameLogic.BuyUpgrade(s, artifactRecCost, u => u with { HasArtifactRec = true });
            if (s.Upgrades.HasArtifactRec) ShowNotification("✨ 공예품 레시피 획득!");
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

    private void DrawShopRow(string label, int cost, System.Action onBuy, bool repeatable = false)
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