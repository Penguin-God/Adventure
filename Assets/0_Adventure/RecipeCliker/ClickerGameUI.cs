using UnityEngine;

// ==========================================
// [상태 및 UI 관리자] 부수효과 처리 구역
// ==========================================
public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 기본 확률 설정")]
    [Range(0f, 100f)] public float forestTier2Chance = 15.0f;
    [Range(0f, 100f)] public float forestTier3Chance = 5.0f;

    [Header("⛰️ 광산 기본 확률 설정")]
    [Range(0f, 100f)] public float mineTier2Chance = 20.0f;
    [Range(0f, 100f)] public float mineTier3Chance = 2.0f;

    [Header("💰 기본 판매 가격")]
    public int t1Price = 10;
    public int t2Price = 150;
    public int t3Price = 2000;
    public int ultimatePrice = 50000;

    // ✨ 초기 상태 세팅 (업그레이드는 0레벨로 시작)
    private ClickerState currentState = new ClickerState(
        new TerrainState(0, 0, 0),
        new TerrainState(0, 0, 0),
        0, 0, new UpgradeState(0, 0, 0)
    );

    // ✨ 판매 효율(5%)이 적용된 최종 가격을 계산하는 람다식
    private int Boosted(int basePrice) => Mathf.RoundToInt(basePrice * (1f + currentState.Upgrades.SellBonus * 0.05f));

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));

        GUILayout.BeginHorizontal();
        GUILayout.Label("🛠️ 지형 조합 클리커 - 상점 및 업그레이드 시스템", GUILayout.Width(450));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {currentState.Money:N0} G", GUILayout.Width(300));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // 1. 인벤토리 구역 (✨ 효율이 적용된 Boosted 가격을 넘겨줍니다)
        GUILayout.BeginHorizontal();
        DrawTerrainInventory("🌲 숲 인벤토리", currentState.Forest,
            () => currentState = ClickerLogic.SellForest(currentState, t => TerrainLogic.SellTier1(t, Boosted(t1Price))),
            () => currentState = ClickerLogic.SellForest(currentState, t => TerrainLogic.SellTier2(t, Boosted(t2Price))),
            () => currentState = ClickerLogic.SellForest(currentState, t => TerrainLogic.SellTier3(t, Boosted(t3Price)))
        );
        GUILayout.Space(20);
        DrawTerrainInventory("⛰️ 광산 인벤토리", currentState.Mine,
            () => currentState = ClickerLogic.SellMine(currentState, t => TerrainLogic.SellTier1(t, Boosted(t1Price))),
            () => currentState = ClickerLogic.SellMine(currentState, t => TerrainLogic.SellTier2(t, Boosted(t2Price))),
            () => currentState = ClickerLogic.SellMine(currentState, t => TerrainLogic.SellTier3(t, Boosted(t3Price)))
        );
        GUILayout.EndHorizontal();
        GUILayout.Space(30);

        // 2. 행동 구역 (✨ 상점에서 올린 추가 확률을 더해서 넘겨줍니다)
        GUILayout.BeginHorizontal();
        DrawTerrainActions(
            "🌲 숲에서 행동", currentState.Forest,
            forestTier2Chance + currentState.Upgrades.Tier2Prob,
            forestTier3Chance + currentState.Upgrades.Tier3Prob,
            (random, t2, t3) => currentState = ClickerLogic.UpdateForest(currentState, t => TerrainLogic.Gather(t, random, t2, t3)),
            () => currentState = ClickerLogic.UpdateForest(currentState, TerrainLogic.CraftTier2),
            () => currentState = ClickerLogic.UpdateForest(currentState, TerrainLogic.CraftTier3)
        );
        GUILayout.Space(20);
        DrawTerrainActions(
            "⛰️ 광산에서 행동", currentState.Mine,
            mineTier2Chance + currentState.Upgrades.Tier2Prob,
            mineTier3Chance + currentState.Upgrades.Tier3Prob,
            (random, t2, t3) => currentState = ClickerLogic.UpdateMine(currentState, t => TerrainLogic.Gather(t, random, t2, t3)),
            () => currentState = ClickerLogic.UpdateMine(currentState, TerrainLogic.CraftTier2),
            () => currentState = ClickerLogic.UpdateMine(currentState, TerrainLogic.CraftTier3)
        );
        GUILayout.EndHorizontal();
        GUILayout.Space(30);

        // 3. 상점 및 궁극기 구역 (나란히 배치)
        GUILayout.BeginHorizontal();
        DrawShopSection();
        GUILayout.Space(20);
        DrawUltimateSection();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // ==========================================
    // 🛒 신규: 상점 렌더링
    // ==========================================
    private void DrawShopSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 2f - 60));
        GUILayout.Label("🛒 상점 (업그레이드)");
        GUILayout.Space(10);

        // 레벨이 오를수록 가격이 비싸집니다 (기본비용 * (레벨+1))
        int t2Cost = 1000 * (currentState.Upgrades.Tier2Prob + 1);
        int t3Cost = 5000 * (currentState.Upgrades.Tier3Prob + 1);
        int sellCost = 2000 * (currentState.Upgrades.SellBonus + 1);

        DrawUpgradeRow($"T2 획득 확률 +1% (현재 +{currentState.Upgrades.Tier2Prob}%)", t2Cost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t2Cost, u => u with { Tier2Prob = u.Tier2Prob + 1 }));

        DrawUpgradeRow($"T3 획득 확률 +1% (현재 +{currentState.Upgrades.Tier3Prob}%)", t3Cost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t3Cost, u => u with { Tier3Prob = u.Tier3Prob + 1 }));

        DrawUpgradeRow($"전체 판매 효율 +5% (현재 +{currentState.Upgrades.SellBonus * 5}%)", sellCost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, sellCost, u => u with { SellBonus = u.SellBonus + 1 }));

        GUILayout.EndVertical();
    }

    private void DrawUltimateSection()
    {
        GUILayout.BeginVertical("box");
        DrawResourceRow("🌟 궁극의 아티팩트", currentState.UltimateItem, Boosted(ultimatePrice),
            () => currentState = ClickerLogic.SellUltimate(currentState, Boosted(ultimatePrice)));
        GUILayout.Space(10);
        DrawCraftButton(
            "✨ 궁극의 아티팩트 조합\n(숲 T3 [1] + 광산 T3 [1])",
            ClickerLogic.CanCraftUltimate(currentState),
            () => currentState = ClickerLogic.CraftUltimate(currentState)
        );
        GUILayout.EndVertical();
    }

    // --- [기존 UI 렌더링 함수들] ---

    private void DrawUpgradeRow(string label, int cost, System.Action onBuy)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        GUILayout.FlexibleSpace();
        GUI.enabled = currentState.Money >= cost;
        if (GUILayout.Button($"💰 구매 ({cost:N0}G)", GUILayout.Width(180))) onBuy?.Invoke();
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    private void DrawTerrainInventory(string title, TerrainState state, System.Action sellT1, System.Action sellT2, System.Action sellT3)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);
        DrawResourceRow("기본 (T1)", state.Tier1, Boosted(t1Price), sellT1);
        DrawResourceRow("가공 (T2)", state.Tier2, Boosted(t2Price), sellT2);
        DrawResourceRow("고급 (T3)", state.Tier3, Boosted(t3Price), sellT3);
        GUILayout.EndVertical();
    }

    private void DrawTerrainActions(
        string title, TerrainState state, float t2Chance, float t3Chance,
        System.Action<float, float, float> onGather,
        System.Action onCraftT2,
        System.Action onCraftT3)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);

        float t1Chance = 100f - (t2Chance + t3Chance);
        if (GUILayout.Button($"⛏️ 채집\n(T3 {t3Chance}% / T2 {t2Chance}% / T1 {t1Chance}%)", GUILayout.Height(80)))
        {
            float rawRandomValue = UnityEngine.Random.Range(0f, 100f);
            onGather?.Invoke(rawRandomValue, t2Chance, t3Chance);
        }
        GUILayout.Space(10);

        DrawCraftButton("🔨 T2 조합 (T1 100개)", TerrainLogic.CanCraftTier2(state), onCraftT2);
        DrawCraftButton("✨ T3 조합 (T2 100개)", TerrainLogic.CanCraftTier3(state), onCraftT3);

        GUILayout.EndVertical();
    }

    private void DrawResourceRow(string name, int amount, int price = 0, System.Action onSell = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(150));
        GUILayout.Label($"{amount} 개", GUILayout.Width(100));
        GUILayout.FlexibleSpace();

        if (price > 0)
        {
            GUI.enabled = amount > 0;
            if (GUILayout.Button($"💰 판매 (+{price:N0}G)", GUILayout.Width(180))) onSell?.Invoke();
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
    }

    private void DrawCraftButton(string label, bool canCraft, System.Action onCraft)
    {
        GUI.enabled = canCraft;
        if (GUILayout.Button(label, GUILayout.Height(60))) onCraft?.Invoke();
        GUI.enabled = true;
        GUILayout.Space(10);
    }
}