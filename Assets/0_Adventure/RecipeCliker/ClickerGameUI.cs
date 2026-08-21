using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 확률 (가공된 목재, 수액, 레시피)")]
    [Range(0f, 100f)] public float woodChance = 5.0f;
    [Range(0f, 100f)] public float sapChance = 15.0f;
    [Range(0f, 100f)] public float forestRecChance = 2.0f;

    [Header("⛰️ 광산 확률 (특수 합금, 은광석, 레시피)")]
    [Range(0f, 100f)] public float alloyChance = 2.0f;
    [Range(0f, 100f)] public float silverChance = 20.0f;
    [Range(0f, 100f)] public float mineRecChance = 1.0f;

    [Header("💰 자원 판매 가격")]
    public int logPrice = 10; public int sapPrice = 50; public int woodPrice = 200;
    public int ironPrice = 15; public int silverPrice = 80; public int alloyPrice = 300;
    public int artifactPrice = 5000;

    [Header("🛒 상점 비용")]
    public int mineUnlockCost = 2000;
    public int woodRecCost = 1000; public int alloyRecCost = 2500; public int artifactRecCost = 10000;
    public int endingDeedCost = 500000;

    private ClickerState s = new ClickerState(new Inventory(0, 0, 0, 0, 0, 0, 0), 0, new UpgradeState(0));
    private int Boosted(int baseP) => Mathf.RoundToInt(baseP * (1f + s.Upgrades.SellBonus * 0.05f));

    // ✨ 실수로 날려버렸던 알림 시스템 완벽 복구
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
        GUILayout.Label("🛠️ 직관적 조합 클리커 - 숲&광산 융합 레이아웃", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        // ==========================================
        // ✨ 레이아웃 2단 분리 (좌측: 지형 조합 / 우측: 상점)
        // ==========================================
        GUILayout.BeginHorizontal();

        // 🟢 [좌측 구역] 숲과 광산을 묶는 거대한 컨테이너
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.66f - 30));

        GUILayout.BeginHorizontal();

        // --- 1. 🌲 숲 구역 ---
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 20));
        DrawInventoryBox("🌲 숲 채집물",
            ("통나무", s.Inv.Log, Boosted(logPrice), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = i.Log - 1 }, 1) : (i, 0), Boosted(logPrice)), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = 0 }, i.Log) : (i, 0), Boosted(logPrice))),
            ("수액", s.Inv.Sap, Boosted(sapPrice), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = i.Sap - 1 }, 1) : (i, 0), Boosted(sapPrice)), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = 0 }, i.Sap) : (i, 0), Boosted(sapPrice))),
            ("가공된 목재", s.Inv.Wood, Boosted(woodPrice), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = i.Wood - 1 }, 1) : (i, 0), Boosted(woodPrice)), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = 0 }, i.Wood) : (i, 0), Boosted(woodPrice)))
        );
        GUILayout.Space(10);
        GUILayout.BeginVertical("box");
        GUILayout.Label("🌲 숲 행동");
        float logChance = 100f - woodChance - sapChance;
        GUILayout.Label($"💎 [드롭] 통나무({logChance:F1}%) / 수액({sapChance}%) / 목재({woodChance}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.Label($"📜 [레시피] 가공된 목재 ({forestRecChance}%) - {(s.Upgrades.HasWoodRec ? "✅" : "❌")}", new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.Space(5);
        if (GUILayout.Button($"⛏️ 숲 채집하기", GUILayout.Height(60)))
        {
            bool hadWoodRec = s.Upgrades.HasWoodRec; // ✨ 획득 전 상태 체크
            s = GameLogic.GatherForest(s, Random.Range(0f, 100f), woodChance, sapChance);
            s = GameLogic.RollForestRecipe(s, Random.Range(0f, 100f), forestRecChance);
            if (!hadWoodRec && s.Upgrades.HasWoodRec) ShowNotification("📜 [가공된 목재 레시피]를 발견했습니다!");
        }
        GUILayout.Space(5);
        if (s.Upgrades.HasWoodRec) DrawCraftButton("🔨 가공된 목재\n[통나무(10) + 수액(3)]", GameLogic.CanCraftWood(s), () => s = GameLogic.CraftWood(s));
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // --- 2. ⛰️ 광산 구역 ---
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 20));
        if (s.Upgrades.IsMineUnlocked)
        {
            DrawInventoryBox("⛰️ 광산 채집물",
                ("철광석", s.Inv.Iron, Boosted(ironPrice), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = i.Iron - 1 }, 1) : (i, 0), Boosted(ironPrice)), () => s = GameLogic.Sell(s, i => i.Iron > 0 ? (i with { Iron = 0 }, i.Iron) : (i, 0), Boosted(ironPrice))),
                ("은광석", s.Inv.Silver, Boosted(silverPrice), () => s = GameLogic.Sell(s, i => i.Silver > 0 ? (i with { Silver = i.Silver - 1 }, 1) : (i, 0), Boosted(silverPrice)), () => s = GameLogic.Sell(s, i => i.Silver > 0 ? (i with { Silver = 0 }, i.Silver) : (i, 0), Boosted(silverPrice))),
                ("특수 합금", s.Inv.Alloy, Boosted(alloyPrice), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = i.Alloy - 1 }, 1) : (i, 0), Boosted(alloyPrice)), () => s = GameLogic.Sell(s, i => i.Alloy > 0 ? (i with { Alloy = 0 }, i.Alloy) : (i, 0), Boosted(alloyPrice)))
            );
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 광산 행동");
            float ironChance = 100f - alloyChance - silverChance;
            GUILayout.Label($"💎 [드롭] 철광석({ironChance:F1}%) / 은광석({silverChance}%) / 합금({alloyChance}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Label($"📜 [레시피] 특수 합금 ({mineRecChance}%) - {(s.Upgrades.HasAlloyRec ? "✅" : "❌")}", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Space(5);
            if (GUILayout.Button($"⛏️ 광산 채집하기", GUILayout.Height(60)))
            {
                bool hadAlloyRec = s.Upgrades.HasAlloyRec;
                s = GameLogic.GatherMine(s, Random.Range(0f, 100f), alloyChance, silverChance);
                s = GameLogic.RollMineRecipe(s, Random.Range(0f, 100f), mineRecChance);
                if (!hadAlloyRec && s.Upgrades.HasAlloyRec) ShowNotification("📜 [특수 합금 레시피]를 발견했습니다!");
            }
            GUILayout.Space(5);
            if (s.Upgrades.HasAlloyRec) DrawCraftButton("🔨 특수 합금\n[철광석(10) + 은광석(3)]", GameLogic.CanCraftAlloy(s), () => s = GameLogic.CraftAlloy(s));
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 미개척 구역");
            GUILayout.FlexibleSpace();
            DrawCraftButton($"🔒 광산 개척하기 ({mineUnlockCost:N0}G)", s.Money >= mineUnlockCost, () => s = GameLogic.BuyUpgrade(s, mineUnlockCost, u => u with { IsMineUnlocked = true }));
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal(); // 숲과 광산의 수평 정렬 끝

        GUILayout.Space(15);

        // --- ✨ 3. 공예품 조합 (숲과 광산의 하단을 길게 통합) ---
        if (s.Upgrades.HasArtifactRec)
        {
            DrawCraftButton("✨ 궁극의 공예품 조합 ✨\n[가공된 목재(1) + 특수 합금(1)]", GameLogic.CanCraftArtifact(s), () => s = GameLogic.CraftArtifact(s), 80);
        }

        GUILayout.EndVertical(); // 🟢 좌측 구역 끝

        GUILayout.Space(10);

        // 🔵 [우측 구역] 상점 및 결과물 보관함
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.33f - 30));

        DrawInventoryBox("✨ 특수 자원 보관함",
            ("공예품", s.Inv.Artifact, Boosted(artifactPrice), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = i.Artifact - 1 }, 1) : (i, 0), Boosted(artifactPrice)), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = 0 }, i.Artifact) : (i, 0), Boosted(artifactPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점");

        int upgCost = 1000 * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"판매 효율 +10% (현재 +{s.Upgrades.SellBonus * 10}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }), true);
        GUILayout.Space(10);

        if (!s.Upgrades.HasWoodRec) DrawShopRow($"📜 가공된 목재 레시피", woodRecCost, () => s = GameLogic.BuyUpgrade(s, woodRecCost, u => u with { HasWoodRec = true }));
        if (!s.Upgrades.HasAlloyRec) DrawShopRow($"📜 특수 합금 레시피", alloyRecCost, () => s = GameLogic.BuyUpgrade(s, alloyRecCost, u => u with { HasAlloyRec = true }));

        if (!s.Upgrades.HasArtifactRec) DrawShopRow($"✨ 공예품 레시피", artifactRecCost, () => {
            s = GameLogic.BuyUpgrade(s, artifactRecCost, u => u with { HasArtifactRec = true });
            if (s.Upgrades.HasArtifactRec) ShowNotification("✨ [공예품 레시피]를 구매했습니다!");
        });

        // 땅문서는 여전히 조용히 배치됩니다.
        if (!s.Upgrades.HasEndingDeed) DrawShopRow($"👑 영주의 땅문서", endingDeedCost, () => s = GameLogic.BuyUpgrade(s, endingDeedCost, u => u with { HasEndingDeed = true }));

        GUILayout.EndVertical();

        GUILayout.EndVertical(); // 🔵 우측 구역 끝

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ==================== 🌟 부활한 알림 UI 렌더링 ====================
        if (Time.time < notificationEndTime)
        {
            GUIStyle notifStyle = new GUIStyle(GUI.skin.box);
            notifStyle.fontSize = 26;
            notifStyle.normal.textColor = Color.yellow;
            notifStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Box(new Rect(Screen.width / 2f - 300, Screen.height / 2f - 50, 600, 100), notificationMsg, notifStyle);
        }
    }

    // --- [UI 재사용 헬퍼 함수들] ---
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

    // ✨ 높이(Height) 매개변수를 추가하여 거대한 버튼을 만들 수 있게 확장했습니다.
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
        if (GUILayout.Button($"💰 {cost:N0}G", GUILayout.Width(120))) onBuy?.Invoke();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }
}