using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 확률 (자원2, 자원3)")]
    [Range(0f, 100f)] public float fT2 = 15.0f; [Range(0f, 100f)] public float fT3 = 5.0f;

    [Header("⛰️ 광산 확률 (자원5, 자원6)")]
    [Range(0f, 100f)] public float mT2 = 20.0f; [Range(0f, 100f)] public float mT3 = 2.0f;

    [Header("💰 자원 판매 가격")]
    public int[] prices = { 0, 10, 20, 50, 15, 30, 80, 500, 300, 1000, 50000 }; // 0번 인덱스는 안 씀 (1~10번)

    [Header("🛒 해금 및 레시피 비용")]
    public int mineUnlockCost = 2000;
    public int rec8Cost = 1000, rec7Cost = 3000, rec9Cost = 5000;

    private ClickerState s = new ClickerState(new Inventory(0, 0, 0, 0, 0, 0, 0, 0, 0, 0), 0, new UpgradeState(0, 0, 0));

    // 판매 보너스 적용 가격
    private int Boosted(int baseP) => Mathf.RoundToInt(baseP * (1f + s.Upgrades.SellBonus * 0.05f));

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 18; GUI.skin.button.fontSize = 18;
        GUILayout.BeginArea(new Rect(30, 30, Screen.width - 60, Screen.height - 60));

        GUILayout.BeginHorizontal();
        GUILayout.Label("🛠️ 지형 크로스 조합 클리커", GUILayout.Width(400));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        // ==================== 1. 🌲 숲 구역 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
        DrawInventoryBox("🌲 숲 채집물",
            ("자원 1", s.Inv.I1, Boosted(prices[1]), () => s = GameLogic.Sell(s, i => i.I1 > 0 ? (i with { I1 = i.I1 - 1 }, 1) : (i, 0), Boosted(prices[1]))),
            ("자원 2", s.Inv.I2, Boosted(prices[2]), () => s = GameLogic.Sell(s, i => i.I2 > 0 ? (i with { I2 = i.I2 - 1 }, 1) : (i, 0), Boosted(prices[2]))),
            ("자원 3", s.Inv.I3, Boosted(prices[3]), () => s = GameLogic.Sell(s, i => i.I3 > 0 ? (i with { I3 = i.I3 - 1 }, 1) : (i, 0), Boosted(prices[3])))
        );
        GUILayout.Space(10);
        GUILayout.BeginVertical("box");
        GUILayout.Label("🌲 숲 행동");
        if (GUILayout.Button($"⛏️ 채집 (자원 1, 2, 3)", GUILayout.Height(60))) s = GameLogic.GatherForest(s, Random.Range(0f, 100f), fT2 + s.Upgrades.Tier2Prob, fT3 + s.Upgrades.Tier3Prob);

        // 숲 전용 조합식 (레시피를 샀을 때만 보임)
        if (s.Upgrades.HasRec8) DrawCraftButton("🔨 조합 8 [자원 1(10) + 자원 2(3)]", GameLogic.CanCraft8(s), () => s = GameLogic.Craft8(s));
        if (s.Upgrades.HasRec7) DrawCraftButton("🔨 조합 7 [자원 3(1) + 자원 4(10) + 자원 5(3)]", GameLogic.CanCraft7(s), () => s = GameLogic.Craft7(s));
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // ==================== 2. ⛰️ 광산 구역 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
        if (s.Upgrades.IsMineUnlocked)
        {
            DrawInventoryBox("⛰️ 광산 채집물",
                ("자원 4", s.Inv.I4, Boosted(prices[4]), () => s = GameLogic.Sell(s, i => i.I4 > 0 ? (i with { I4 = i.I4 - 1 }, 1) : (i, 0), Boosted(prices[4]))),
                ("자원 5", s.Inv.I5, Boosted(prices[5]), () => s = GameLogic.Sell(s, i => i.I5 > 0 ? (i with { I5 = i.I5 - 1 }, 1) : (i, 0), Boosted(prices[5]))),
                ("자원 6", s.Inv.I6, Boosted(prices[6]), () => s = GameLogic.Sell(s, i => i.I6 > 0 ? (i with { I6 = i.I6 - 1 }, 1) : (i, 0), Boosted(prices[6])))
            );
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 광산 행동");
            if (GUILayout.Button($"⛏️ 채집 (자원 4, 5, 6)", GUILayout.Height(60))) s = GameLogic.GatherMine(s, Random.Range(0f, 100f), mT2 + s.Upgrades.Tier2Prob, mT3 + s.Upgrades.Tier3Prob);

            // 광산 전용 조합식
            if (s.Upgrades.HasRec9) DrawCraftButton("🔨 조합 9 [자원 8(1) + 자원 6(1)]", GameLogic.CanCraft9(s), () => s = GameLogic.Craft9(s));
            // 궁극 레시피는 상점 구매 없이 기본 개방
            DrawCraftButton("✨ 궁극 조합 10 [자원 7(1) + 자원 8(1) + 자원 9(1)]", GameLogic.CanCraft10(s), () => s = GameLogic.Craft10(s));
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

        // ==================== 3. ✨ 상점 및 결과물 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
        DrawInventoryBox("✨ 특수 자원 보관함",
            ("자원 7", s.Inv.I7, Boosted(prices[7]), () => s = GameLogic.Sell(s, i => i.I7 > 0 ? (i with { I7 = i.I7 - 1 }, 1) : (i, 0), Boosted(prices[7]))),
            ("자원 8", s.Inv.I8, Boosted(prices[8]), () => s = GameLogic.Sell(s, i => i.I8 > 0 ? (i with { I8 = i.I8 - 1 }, 1) : (i, 0), Boosted(prices[8]))),
            ("자원 9", s.Inv.I9, Boosted(prices[9]), () => s = GameLogic.Sell(s, i => i.I9 > 0 ? (i with { I9 = i.I9 - 1 }, 1) : (i, 0), Boosted(prices[9]))),
            ("궁극 자원 10", s.Inv.I10, Boosted(prices[10]), () => s = GameLogic.Sell(s, i => i.I10 > 0 ? (i with { I10 = i.I10 - 1 }, 1) : (i, 0), Boosted(prices[10])))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점");
        int upgCost = 2000 * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"판매 효율 +5% (현재 +{s.Upgrades.SellBonus * 5}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }), true);
        GUILayout.Space(10);

        if (!s.Upgrades.HasRec8) DrawShopRow($"📜 레시피 8 구매", rec8Cost, () => s = GameLogic.BuyUpgrade(s, rec8Cost, u => u with { HasRec8 = true }));
        if (!s.Upgrades.HasRec7) DrawShopRow($"📜 레시피 7 구매", rec7Cost, () => s = GameLogic.BuyUpgrade(s, rec7Cost, u => u with { HasRec7 = true }));
        if (!s.Upgrades.HasRec9) DrawShopRow($"📜 레시피 9 구매", rec9Cost, () => s = GameLogic.BuyUpgrade(s, rec9Cost, u => u with { HasRec9 = true }));
        GUILayout.EndVertical();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // --- [UI 재사용 헬퍼 함수들] ---

    // ✨ 아이템 여러 개를 한 번에 렌더링하도록 params 튜플 활용
    private void DrawInventoryBox(string title, params (string name, int amount, int price, System.Action onSell)[] items)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(5);
        foreach (var item in items)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.name, GUILayout.Width(100));
            GUILayout.Label($"{item.amount} 개", GUILayout.Width(70));
            GUILayout.FlexibleSpace();
            GUI.enabled = item.amount > 0;
            if (GUILayout.Button($"💰 +{item.price:N0}G", GUILayout.Width(110))) item.onSell?.Invoke();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
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
        if (GUILayout.Button($"💰 {cost:N0}G", GUILayout.Width(110))) onBuy?.Invoke();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }
}