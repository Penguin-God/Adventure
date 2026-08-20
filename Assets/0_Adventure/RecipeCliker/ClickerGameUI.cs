using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 확률 (자원2, 자원3, 레시피8, 레시피7)")]
    [Range(0f, 100f)] public float fT2 = 15.0f; [Range(0f, 100f)] public float fT3 = 5.0f;
    [Range(0f, 100f)] public float fRec8Prob = 2.0f; [Range(0f, 100f)] public float fRec7Prob = 0.5f;

    [Header("⛰️ 광산 확률 (자원5, 자원6, 레시피9, 궁극레시피10)")]
    [Range(0f, 100f)] public float mT2 = 20.0f; [Range(0f, 100f)] public float mT3 = 2.0f;
    [Range(0f, 100f)] public float mRec9Prob = 1.0f;
    [Range(0f, 100f)] public float mRec10Prob = 0.1f;

    [Header("💰 자원 판매 가격")]
    public int[] prices = { 0, 10, 20, 50, 15, 30, 80, 500, 300, 1000, 50000 };

    [Header("🛒 해금 및 상점 레시피 비용")]
    public int mineUnlockCost = 2000;
    public int rec8Cost = 1000, rec7Cost = 3000, rec9Cost = 5000;

    private ClickerState s = new ClickerState(new Inventory(0, 0, 0, 0, 0, 0, 0, 0, 0, 0), 0, new UpgradeState(0, 0, 0));
    private int Boosted(int baseP) => Mathf.RoundToInt(baseP * (1f + s.Upgrades.SellBonus * 0.05f));

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 16; GUI.skin.button.fontSize = 16;
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));

        GUILayout.BeginHorizontal();
        GUILayout.Label("🛠️ 지형 크로스 조합 클리커 - 궁극 레시피 드롭 & 확률 표기", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {s.Money:N0} G", GUILayout.Width(250));
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        // ==================== 1. 🌲 숲 구역 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));

        DrawInventoryBox("🌲 숲 채집물",
            ("자원 1", s.Inv.I1, Boosted(prices[1]), () => s = GameLogic.Sell(s, i => i.I1 > 0 ? (i with { I1 = i.I1 - 1 }, 1) : (i, 0), Boosted(prices[1])), () => s = GameLogic.Sell(s, i => i.I1 > 0 ? (i with { I1 = 0 }, i.I1) : (i, 0), Boosted(prices[1]))),
            ("자원 2", s.Inv.I2, Boosted(prices[2]), () => s = GameLogic.Sell(s, i => i.I2 > 0 ? (i with { I2 = i.I2 - 1 }, 1) : (i, 0), Boosted(prices[2])), () => s = GameLogic.Sell(s, i => i.I2 > 0 ? (i with { I2 = 0 }, i.I2) : (i, 0), Boosted(prices[2]))),
            ("자원 3", s.Inv.I3, Boosted(prices[3]), () => s = GameLogic.Sell(s, i => i.I3 > 0 ? (i with { I3 = i.I3 - 1 }, 1) : (i, 0), Boosted(prices[3])), () => s = GameLogic.Sell(s, i => i.I3 > 0 ? (i with { I3 = 0 }, i.I3) : (i, 0), Boosted(prices[3])))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🌲 숲 행동 및 정보");

        float fT2Act = fT2 + s.Upgrades.Tier2Prob; float fT3Act = fT3 + s.Upgrades.Tier3Prob; float fT1Act = 100f - fT2Act - fT3Act;
        GUILayout.Label($"💎 [광물 확률] 자원1({fT1Act}%) / 자원2({fT2Act}%) / 자원3({fT3Act}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
        GUILayout.Space(5);

        GUILayout.Label($"📜 조합 8 레시피 ({fRec8Prob}%) - {(s.Upgrades.HasRec8 ? "✅ 획득" : "❌ 미획득")}");
        GUILayout.Label($"📜 조합 7 레시피 ({fRec7Prob}%) - {(s.Upgrades.HasRec7 ? "✅ 획득" : "❌ 미획득")}");
        GUILayout.Space(5);

        if (GUILayout.Button($"⛏️ 숲 채집하기", GUILayout.Height(60)))
        {
            s = GameLogic.GatherForest(s, Random.Range(0f, 100f), fT2Act, fT3Act);
            s = GameLogic.RollForestRecipes(s, Random.Range(0f, 100f), fRec8Prob, Random.Range(0f, 100f), fRec7Prob);
        }

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
                ("자원 4", s.Inv.I4, Boosted(prices[4]), () => s = GameLogic.Sell(s, i => i.I4 > 0 ? (i with { I4 = i.I4 - 1 }, 1) : (i, 0), Boosted(prices[4])), () => s = GameLogic.Sell(s, i => i.I4 > 0 ? (i with { I4 = 0 }, i.I4) : (i, 0), Boosted(prices[4]))),
                ("자원 5", s.Inv.I5, Boosted(prices[5]), () => s = GameLogic.Sell(s, i => i.I5 > 0 ? (i with { I5 = i.I5 - 1 }, 1) : (i, 0), Boosted(prices[5])), () => s = GameLogic.Sell(s, i => i.I5 > 0 ? (i with { I5 = 0 }, i.I5) : (i, 0), Boosted(prices[5]))),
                ("자원 6", s.Inv.I6, Boosted(prices[6]), () => s = GameLogic.Sell(s, i => i.I6 > 0 ? (i with { I6 = i.I6 - 1 }, 1) : (i, 0), Boosted(prices[6])), () => s = GameLogic.Sell(s, i => i.I6 > 0 ? (i with { I6 = 0 }, i.I6) : (i, 0), Boosted(prices[6])))
            );
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            GUILayout.Label("⛰️ 광산 행동 및 정보");

            float mT2Act = mT2 + s.Upgrades.Tier2Prob; float mT3Act = mT3 + s.Upgrades.Tier3Prob; float mT1Act = 100f - mT2Act - mT3Act;
            GUILayout.Label($"💎 [광물 확률] 자원4({mT1Act}%) / 자원5({mT2Act}%) / 자원6({mT3Act}%)", new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUILayout.Space(5);

            GUILayout.Label($"📜 조합 9 레시피 ({mRec9Prob}%) - {(s.Upgrades.HasRec9 ? "✅ 획득" : "❌ 미획득")}");
            GUILayout.Label($"✨ 궁극 조합 10 레시피 ({mRec10Prob}%) - {(s.Upgrades.HasRec10 ? "✅ 획득" : "❌ 미획득")}");
            GUILayout.Space(5);

            if (GUILayout.Button($"⛏️ 광산 채집하기", GUILayout.Height(60)))
            {
                s = GameLogic.GatherMine(s, Random.Range(0f, 100f), mT2Act, mT3Act);
                s = GameLogic.RollMineRecipes(s, Random.Range(0f, 100f), mRec9Prob, Random.Range(0f, 100f), mRec10Prob);
            }

            if (s.Upgrades.HasRec9) DrawCraftButton("🔨 조합 9 [자원 8(1) + 자원 6(1)]", GameLogic.CanCraft9(s), () => s = GameLogic.Craft9(s));
            if (s.Upgrades.HasRec10) DrawCraftButton("✨ 궁극 조합 10 [자원 7(1) + 자원 8(1) + 자원 9(1)]", GameLogic.CanCraft10(s), () => s = GameLogic.Craft10(s));
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

        // ==================== 3. ✨ 상점 및 특수 자원 ====================
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3f - 30));
        DrawInventoryBox("✨ 특수 자원 보관함",
            ("자원 7", s.Inv.I7, Boosted(prices[7]), () => s = GameLogic.Sell(s, i => i.I7 > 0 ? (i with { I7 = i.I7 - 1 }, 1) : (i, 0), Boosted(prices[7])), () => s = GameLogic.Sell(s, i => i.I7 > 0 ? (i with { I7 = 0 }, i.I7) : (i, 0), Boosted(prices[7]))),
            ("자원 8", s.Inv.I8, Boosted(prices[8]), () => s = GameLogic.Sell(s, i => i.I8 > 0 ? (i with { I8 = i.I8 - 1 }, 1) : (i, 0), Boosted(prices[8])), () => s = GameLogic.Sell(s, i => i.I8 > 0 ? (i with { I8 = 0 }, i.I8) : (i, 0), Boosted(prices[8]))),
            ("자원 9", s.Inv.I9, Boosted(prices[9]), () => s = GameLogic.Sell(s, i => i.I9 > 0 ? (i with { I9 = i.I9 - 1 }, 1) : (i, 0), Boosted(prices[9])), () => s = GameLogic.Sell(s, i => i.I9 > 0 ? (i with { I9 = 0 }, i.I9) : (i, 0), Boosted(prices[9]))),
            ("궁극자원", s.Inv.I10, Boosted(prices[10]), () => s = GameLogic.Sell(s, i => i.I10 > 0 ? (i with { I10 = i.I10 - 1 }, 1) : (i, 0), Boosted(prices[10])), () => s = GameLogic.Sell(s, i => i.I10 > 0 ? (i with { I10 = 0 }, i.I10) : (i, 0), Boosted(prices[10])))
        );
        GUILayout.Space(10);

        GUILayout.BeginVertical("box");
        GUILayout.Label("🛒 상점 (레시피 즉시 구매)");
        int upgCost = 2000 * (s.Upgrades.SellBonus + 1);
        DrawShopRow($"효율 +5% (현재 +{s.Upgrades.SellBonus * 5}%)", upgCost, () => s = GameLogic.BuyUpgrade(s, upgCost, u => u with { SellBonus = u.SellBonus + 1 }), true);
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

    private void DrawInventoryBox(string title, params (string name, int amount, int price, System.Action onSell, System.Action onSellAll)[] items)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(5);
        foreach (var item in items)
        {
            GUILayout.BeginHorizontal();
            // ✨ 텍스트 라벨과 버튼의 Width를 대폭 늘려서 큰 금액도 넉넉하게 담습니다.
            GUILayout.Label(item.name, GUILayout.Width(70));
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
        // ✨ 상점 버튼 너비도 여유롭게 키웠습니다.
        if (GUILayout.Button($"💰 {cost:N0}G", GUILayout.Width(130))) onBuy?.Invoke();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }
}