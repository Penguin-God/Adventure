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

    private ClickerState s = new ClickerState(new Inventory(0, 0, 0, 0, 0, 0, 0), 0, new UpgradeState(0));
    private int Boosted(int baseP) => Mathf.RoundToInt(baseP * (1f + s.Upgrades.SellBonus * 0.05f));

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
        GUILayout.Label("🛠️ 직관적 조합 클리커 - 조합 비율 변경 & 확률 표기", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        // ==================== 1. 🌲 숲 구역 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));

        DrawInventoryBox("🌲 숲 채집물",
            ("통나무", s.Inv.Log, Boosted(logPrice), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = i.Log - 1 }, 1) : (i, 0), Boosted(logPrice)), () => s = GameLogic.Sell(s, i => i.Log > 0 ? (i with { Log = 0 }, i.Log) : (i, 0), Boosted(logPrice))),
            ("수액", s.Inv.Sap, Boosted(sapPrice), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = i.Sap - 1 }, 1) : (i, 0), Boosted(sapPrice)), () => s = GameLogic.Sell(s, i => i.Sap > 0 ? (i with { Sap = 0 }, i.Sap) : (i, 0), Boosted(sapPrice))),
            ("가공된 목재", s.Inv.Wood, Boosted(woodPrice), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = i.Wood - 1 }, 1) : (i, 0), Boosted(woodPrice)), () => s = GameLogic.Sell(s, i => i.Wood > 0 ? (i with { Wood = 0 }, i.Wood) : (i, 0), Boosted(woodPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🌲 숲 행동");

        // ✨ 기본 재료(통나무)의 드롭 확률을 계산하여 표기합니다.
        float logChance = 100f - woodChance - sapChance;
        GUILayout.Label($"💎 [드롭] 통나무({logChance:F1}%) / 수액({sapChance}%) / 가공된 목재({woodChance}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.Label($"📜 [레시피 드롭] 가공된 목재 ({forestRecChance}%) - {(s.Upgrades.HasWoodRec ? "✅ 획득" : "❌ 미획득")}", new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.Space(5);

        if (GUILayout.Button($"⛏️ 숲 채집하기", GUILayout.Height(60)))
        {
            bool hadRecipe = s.Upgrades.HasWoodRec;
            s = GameLogic.GatherForest(s, Random.Range(0f, 100f), woodChance, sapChance);
            s = GameLogic.RollForestRecipe(s, Random.Range(0f, 100f), forestRecChance);
            if (!hadRecipe && s.Upgrades.HasWoodRec) ShowNotification("📜 [가공된 목재 레시피]를 발견했습니다!");
        }

        GUILayout.Space(5);
        // ✨ 조합 버튼 텍스트 변경
        if (s.Upgrades.HasWoodRec) DrawCraftButton("🔨 가공된 목재 [통나무(10) + 수액(3)]", GameLogic.CanCraftWood(s), () => s = GameLogic.CraftWood(s));
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // ==================== 2. ⛰️ 광산 구역 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
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

            // ✨ 기본 재료(철광석)의 드롭 확률을 계산하여 표기합니다.
            float ironChance = 100f - alloyChance - silverChance;
            GUILayout.Label($"💎 [드롭] 철광석({ironChance:F1}%) / 은광석({silverChance}%) / 합금({alloyChance}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Label($"📜 [레시피 드롭] 특수 합금 ({mineRecChance}%) - {(s.Upgrades.HasAlloyRec ? "✅ 획득" : "❌ 미획득")}", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Space(5);

            if (GUILayout.Button($"⛏️ 광산 채집하기", GUILayout.Height(60)))
            {
                bool hadRecipe = s.Upgrades.HasAlloyRec;
                s = GameLogic.GatherMine(s, Random.Range(0f, 100f), alloyChance, silverChance);
                s = GameLogic.RollMineRecipe(s, Random.Range(0f, 100f), mineRecChance);
                if (!hadRecipe && s.Upgrades.HasAlloyRec) ShowNotification("📜 [특수 합금 레시피]를 발견했습니다!");
            }

            GUILayout.Space(5);
            // ✨ 조합 버튼 텍스트 변경
            if (s.Upgrades.HasAlloyRec) DrawCraftButton("🔨 특수 합금 [철광석(10) + 은광석(3)]", GameLogic.CanCraftAlloy(s), () => s = GameLogic.CraftAlloy(s));
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

        GUILayout.Space(10);

        // ==================== 3. ✨ 상점 및 공예품 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
        DrawInventoryBox("✨ 최종 결과물",
            ("공예품", s.Inv.Artifact, Boosted(artifactPrice), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = i.Artifact - 1 }, 1) : (i, 0), Boosted(artifactPrice)), () => s = GameLogic.Sell(s, i => i.Artifact > 0 ? (i with { Artifact = 0 }, i.Artifact) : (i, 0), Boosted(artifactPrice)))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점");

        int upgCost = 2000 * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"판매 효율 +5% (현재 +{s.Upgrades.SellBonus * 5}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }), true);
        GUILayout.Space(10);

        if (!s.Upgrades.HasWoodRec) DrawShopRow($"📜 가공된 목재 레시피", woodRecCost, () => s = GameLogic.BuyUpgrade(s, woodRecCost, u => u with { HasWoodRec = true }));
        if (!s.Upgrades.HasAlloyRec) DrawShopRow($"📜 특수 합금 레시피", alloyRecCost, () => s = GameLogic.BuyUpgrade(s, alloyRecCost, u => u with { HasAlloyRec = true }));
        // ✨ 공예품 레시피는 상점 전용
        if (!s.Upgrades.HasArtifactRec) DrawShopRow($"✨ 공예품 레시피", artifactRecCost, () => {
            s = GameLogic.BuyUpgrade(s, artifactRecCost, u => u with { HasArtifactRec = true });
            if (s.Upgrades.HasArtifactRec) ShowNotification("✨ [공예품 레시피]를 구매했습니다!");
        });

        GUILayout.Space(15);
        if (s.Upgrades.HasArtifactRec) DrawCraftButton("✨ 공예품 조합\n[가공된 목재(1) + 특수 합금(1)]", GameLogic.CanCraftArtifact(s), () => s = GameLogic.CraftArtifact(s));

        GUILayout.EndVertical();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ==================== 🌟 알림 UI 렌더링 ====================
        if (Time.time < notificationEndTime)
        {
            GUIStyle notifStyle = new GUIStyle(GUI.skin.box);
            notifStyle.fontSize = 26;
            notifStyle.normal.textColor = Color.yellow;
            notifStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Box(new Rect(Screen.width / 2f - 250, Screen.height / 2f - 50, 500, 100), notificationMsg, notifStyle);
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
            GUILayout.Label(item.name, GUILayout.Width(100));
            GUILayout.Label($"{item.amount}개", GUILayout.Width(60));
            GUILayout.FlexibleSpace();

            GUI.enabled = item.amount > 0;
            if (GUILayout.Button($"1개(+{item.price:N0})", GUILayout.Width(110))) item.onSell?.Invoke();
            if (GUILayout.Button($"일괄(+{item.amount * item.price:N0})", GUILayout.Width(130))) item.onSellAll?.Invoke();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        GUILayout.EndVertical();
    }

    private void DrawCraftButton(string label, bool canCraft, System.Action onCraft)
    {
        GUI.enabled = canCraft;
        if (GUILayout.Button(label, GUILayout.Height(50))) onCraft?.Invoke();
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