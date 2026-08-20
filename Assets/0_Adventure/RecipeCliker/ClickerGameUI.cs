using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 기본 확률 설정")]
    [Range(0f, 100f)] public float forestTier2Chance = 15.0f;
    [Range(0f, 100f)] public float forestTier3Chance = 5.0f;

    [Header("⛰️ 광산 기본 확률 설정")]
    [Range(0f, 100f)] public float mineTier2Chance = 20.0f;
    [Range(0f, 100f)] public float mineTier3Chance = 2.0f;

    [Header("📜 레시피 및 연금술 확률 설정")]
    [Range(0f, 100f)] public float recipeDropChance = 1.0f; // 채굴 시 T1->T3 레시피 드롭 확률 (1%)
    [Range(0f, 100f)] public float mixSuccessChance = 70.0f; // 이종 T2 연금술 성공 확률 (70%)
    public int t1ToT3RecipePrice = 20000; // 상점 구매가
    public int t2MixRecipePrice = 10000;  // 상점 연금술 레시피 구매가

    [Header("💰 기본 판매 가격")]
    public int t1Price = 10;
    public int t2Price = 150;
    public int t3Price = 2000;
    public int ultimatePrice = 50000;

    private ClickerState currentState = new ClickerState(
        new TerrainState(0, 0, 0),
        new TerrainState(0, 0, 0),
        0, 0, new UpgradeState(0, 0, 0)
    );

    private int Boosted(int basePrice) => Mathf.RoundToInt(basePrice * (1f + currentState.Upgrades.SellBonus * 0.05f));

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));

        GUILayout.BeginHorizontal();
        GUILayout.Label("🛠️ 지형 조합 클리커 - 레시피 & 상점 연금술", GUILayout.Width(500));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {currentState.Money:N0} G", GUILayout.Width(300));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // 1. 인벤토리 구역
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
        GUILayout.Space(20);

        // 2. 행동 구역 (채집 시 레시피 드롭 체크 포함)
        GUILayout.BeginHorizontal();
        DrawTerrainActions(
            "🌲 숲에서 행동", currentState.Forest,
            forestTier2Chance + currentState.Upgrades.Tier2Prob,
            forestTier3Chance + currentState.Upgrades.Tier3Prob,
            (random, recipeRoll) => {
                currentState = ClickerLogic.UpdateForest(currentState, t => TerrainLogic.Gather(t, random, forestTier2Chance + currentState.Upgrades.Tier2Prob, forestTier3Chance + currentState.Upgrades.Tier3Prob));
                currentState = ClickerLogic.CheckRecipeDrop(currentState, recipeRoll, recipeDropChance);
            },
            () => currentState = ClickerLogic.UpdateForest(currentState, TerrainLogic.CraftTier2),
            () => currentState = ClickerLogic.UpdateForest(currentState, TerrainLogic.CraftTier3),
            () => currentState = ClickerLogic.UpdateForest(currentState, t => TerrainLogic.CraftT1ToT3(t, currentState.Upgrades.HasT1ToT3Recipe))
        );
        GUILayout.Space(20);
        DrawTerrainActions(
            "⛰️ 광산에서 행동", currentState.Mine,
            mineTier2Chance + currentState.Upgrades.Tier2Prob,
            mineTier3Chance + currentState.Upgrades.Tier3Prob,
            (random, recipeRoll) => {
                currentState = ClickerLogic.UpdateMine(currentState, t => TerrainLogic.Gather(t, random, mineTier2Chance + currentState.Upgrades.Tier2Prob, mineTier3Chance + currentState.Upgrades.Tier3Prob));
                currentState = ClickerLogic.CheckRecipeDrop(currentState, recipeRoll, recipeDropChance);
            },
            () => currentState = ClickerLogic.UpdateMine(currentState, TerrainLogic.CraftTier2),
            () => currentState = ClickerLogic.UpdateMine(currentState, TerrainLogic.CraftTier3),
            () => currentState = ClickerLogic.UpdateMine(currentState, t => TerrainLogic.CraftT1ToT3(t, currentState.Upgrades.HasT1ToT3Recipe))
        );
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // 3. 상점 및 궁극기 구역
        GUILayout.BeginHorizontal();
        DrawShopSection();
        GUILayout.Space(20);
        DrawUltimateSection();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawShopSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 2f - 60));
        GUILayout.Label("🛒 상점 & 레시피 해금");
        GUILayout.Space(10);

        int t2Cost = 1000 * (currentState.Upgrades.Tier2Prob + 1);
        int t3Cost = 5000 * (currentState.Upgrades.Tier3Prob + 1);
        int sellCost = 2000 * (currentState.Upgrades.SellBonus + 1);

        DrawUpgradeRow($"T2 획득 확률 +1% (현재 +{currentState.Upgrades.Tier2Prob}%)", t2Cost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t2Cost, u => u with { Tier2Prob = u.Tier2Prob + 1 }));

        DrawUpgradeRow($"T3 획득 확률 +1% (현재 +{currentState.Upgrades.Tier3Prob}%)", t3Cost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t3Cost, u => u with { Tier3Prob = u.Tier3Prob + 1 }));

        DrawUpgradeRow($"전체 판매 효율 +5% (현재 +{currentState.Upgrades.SellBonus * 5}%)", sellCost,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, sellCost, u => u with { SellBonus = u.SellBonus + 1 }));

        GUILayout.Space(10);
        GUILayout.Label("📜 특별 레시피 구매");

        // ✨ 1) T1->T3 직통 레시피 (채굴 중 뽑기로 나오거나, 상점에서 골드로 즉시 구매 가능)
        string t1ToT3Label = currentState.Upgrades.HasT1ToT3Recipe ? "⚡ T1->T3 직통 레시피 (보유 중)" : $"⚡ T1->T3 직통 레시피 ({t1ToT3RecipePrice:N0}G)";
        DrawUpgradeRow(t1ToT3Label, currentState.Upgrades.HasT1ToT3Recipe ? 0 : t1ToT3RecipePrice,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t1ToT3RecipePrice, u => u with { HasT1ToT3Recipe = true }), !currentState.Upgrades.HasT1ToT3Recipe);

        // ✨ 2) 이종 T2 연금술 레시피 (상점 구매)
        string t2MixLabel = currentState.Upgrades.HasT2MixRecipe ? "🧪 이종 T2 연금술 레시피 (보유 중)" : $"🧪 이종 T2 연금술 레시피 ({t2MixRecipePrice:N0}G)";
        DrawUpgradeRow(t2MixLabel, currentState.Upgrades.HasT2MixRecipe ? 0 : t2MixRecipePrice,
            () => currentState = ClickerLogic.BuyUpgrade(currentState, t2MixRecipePrice, u => u with { HasT2MixRecipe = true }), !currentState.Upgrades.HasT2MixRecipe);

        // ✨ 연금술 레시피 해금 시 상점에 연금술 조합 실행 버튼 출력
        if (currentState.Upgrades.HasT2MixRecipe)
        {
            GUILayout.Space(10);
            DrawCraftButton($"🧪 이종 T2 연금술 실행\n(숲 T2 30개 + 광산 T2 30개 -> {mixSuccessChance}% 확률로 T3 1개 획득)", ClickerLogic.CanMixT2(currentState), () => {
                float roll = UnityEngine.Random.Range(0f, 100f);
                bool isForestTarget = UnityEngine.Random.value > 0.5f; // 50% 확률로 숲 T3 또는 광산 T3 결정
                currentState = ClickerLogic.MixT2(currentState, roll, mixSuccessChance, isForestTarget);
            });
        }

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

    private void DrawUpgradeRow(string label, int cost, System.Action onBuy, bool canBuyMore = true)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        GUILayout.FlexibleSpace();
        if (canBuyMore && cost > 0)
        {
            GUI.enabled = currentState.Money >= cost;
            if (GUILayout.Button($"💰 구매", GUILayout.Width(120))) onBuy?.Invoke();
            GUI.enabled = true;
        }
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
        System.Action<float, float> onGather,
        System.Action onCraftT2,
        System.Action onCraftT3,
        System.Action onCraftT1ToT3)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);

        float t1Chance = 100f - (t2Chance + t3Chance);
        if (GUILayout.Button($"⛏️ 채집\n(T3 {t3Chance}% / T2 {t2Chance}% / T1 {t1Chance}%)", GUILayout.Height(70)))
        {
            float rawRandomValue = UnityEngine.Random.Range(0f, 100f);
            float recipeRoll = UnityEngine.Random.Range(0f, 100f);
            onGather?.Invoke(rawRandomValue, recipeRoll);
        }
        GUILayout.Space(10);

        DrawCraftButton("🔨 T2 조합 (T1 100개)", TerrainLogic.CanCraftTier2(state), onCraftT2);
        DrawCraftButton("✨ T3 조합 (T2 100개)", TerrainLogic.CanCraftTier3(state), onCraftT3);

        // ✨ T1->T3 직통 레시피가 해금되었을 때만 행동창에 특수 조합 버튼 표시
        if (currentState.Upgrades.HasT1ToT3Recipe)
        {
            DrawCraftButton("⚡ T1->T3 직통 조합 (T1 3,000개)", TerrainLogic.CanCraftT1ToT3(state, true), onCraftT1ToT3);
        }

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
        if (GUILayout.Button(label, GUILayout.Height(50))) onCraft?.Invoke();
        GUI.enabled = true;
        GUILayout.Space(10);
    }
}