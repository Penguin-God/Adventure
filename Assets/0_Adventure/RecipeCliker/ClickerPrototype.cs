using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record TerrainState(int Tier1, int Tier2, int Tier3);
// ✨ 업그레이드 및 해금된 레시피 상태를 보관하는 불변 레코드
public record UpgradeState(int Tier2Prob, int Tier3Prob, int SellBonus, bool HasT1ToT3Recipe = false, bool HasT2MixRecipe = false);
public record ClickerState(TerrainState Forest, TerrainState Mine, int UltimateItem, int Money, UpgradeState Upgrades);

public static class TerrainLogic
{
    public static TerrainState Gather(TerrainState state, float randomValue, float t2Rate, float t3Rate)
    {
        if (randomValue <= t3Rate) return state with { Tier3 = state.Tier3 + 1 };
        else if (randomValue <= t3Rate + t2Rate) return state with { Tier2 = state.Tier2 + 1 };
        else return state with { Tier1 = state.Tier1 + 1 };
    }

    public static bool CanCraftTier2(TerrainState state) => state.Tier1 >= 100;
    public static bool CanCraftTier3(TerrainState state) => state.Tier2 >= 100;
    // ✨ T1 3000개 -> T3 직통 조합 가능 여부
    public static bool CanCraftT1ToT3(TerrainState state, bool hasRecipe) => hasRecipe && state.Tier1 >= 3000;

    public static TerrainState CraftTier2(TerrainState state) => CanCraftTier2(state) ? state with { Tier1 = state.Tier1 - 100, Tier2 = state.Tier2 + 1 } : state;
    public static TerrainState CraftTier3(TerrainState state) => CanCraftTier3(state) ? state with { Tier2 = state.Tier2 - 100, Tier3 = state.Tier3 + 1 } : state;
    // ✨ T1 3000개를 소모하여 T3 1개 즉시 제작
    public static TerrainState CraftT1ToT3(TerrainState state, bool hasRecipe) => CanCraftT1ToT3(state, hasRecipe) ? state with { Tier1 = state.Tier1 - 3000, Tier3 = state.Tier3 + 1 } : state;

    public static (TerrainState next, int earned) SellTier1(TerrainState state, int price) => state.Tier1 > 0 ? (state with { Tier1 = state.Tier1 - 1 }, price) : (state, 0);
    public static (TerrainState next, int earned) SellTier2(TerrainState state, int price) => state.Tier2 > 0 ? (state with { Tier2 = state.Tier2 - 1 }, price) : (state, 0);
    public static (TerrainState next, int earned) SellTier3(TerrainState state, int price) => state.Tier3 > 0 ? (state with { Tier3 = state.Tier3 - 1 }, price) : (state, 0);
}

public static class ClickerLogic
{
    public static ClickerState UpdateForest(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Forest = updateFunc(state.Forest) };
    public static ClickerState UpdateMine(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Mine = updateFunc(state.Mine) };

    // ✨ 채굴 시 일정 확률로 T1->T3 레시피 획득 체크
    public static ClickerState CheckRecipeDrop(ClickerState state, float dropRoll, float recipeDropChance) =>
        (dropRoll <= recipeDropChance && !state.Upgrades.HasT1ToT3Recipe) ? state with { Upgrades = state.Upgrades with { HasT1ToT3Recipe = true } } : state;

    public static ClickerState SellForest(ClickerState state, Func<TerrainState, (TerrainState next, int earned)> sellFunc) =>
        sellFunc(state.Forest) switch { var (n, e) => state with { Forest = n, Money = state.Money + e } };

    public static ClickerState SellMine(ClickerState state, Func<TerrainState, (TerrainState next, int earned)> sellFunc) =>
        sellFunc(state.Mine) switch { var (n, e) => state with { Mine = n, Money = state.Money + e } };

    public static bool CanCraftUltimate(ClickerState state) => state.Forest.Tier3 >= 1 && state.Mine.Tier3 >= 1;

    public static ClickerState CraftUltimate(ClickerState state) => CanCraftUltimate(state) ?
        state with { Forest = state.Forest with { Tier3 = state.Forest.Tier3 - 1 }, Mine = state.Mine with { Tier3 = state.Mine.Tier3 - 1 }, UltimateItem = state.UltimateItem + 1 } : state;

    public static ClickerState SellUltimate(ClickerState state, int price) =>
        state.UltimateItem > 0 ? state with { UltimateItem = state.UltimateItem - 1, Money = state.Money + price } : state;

    public static ClickerState BuyUpgrade(ClickerState state, int cost, Func<UpgradeState, UpgradeState> upgradeFunc) =>
        state.Money >= cost ? state with { Money = state.Money - cost, Upgrades = upgradeFunc(state.Upgrades) } : state;

    // ✨ 서로 다른 T2 (숲 T2 30개 + 광산 T2 30개) 보유 여부 검사
    public static bool CanMixT2(ClickerState state) => state.Upgrades.HasT2MixRecipe && state.Forest.Tier2 >= 30 && state.Mine.Tier2 >= 30;

    // ✨ 서로 다른 T2 30개씩 소모하여 확률적으로 T3 중 하나(숲 T3 or 광산 T3) 획득
    public static ClickerState MixT2(ClickerState state, float successRoll, float successChance, bool isForestT3Target)
    {
        if (!CanMixT2(state)) return state;
        var nextForest = state.Forest with { Tier2 = state.Forest.Tier2 - 30 };
        var nextMine = state.Mine with { Tier2 = state.Mine.Tier2 - 30 };
        if (successRoll > successChance) return state with { Forest = nextForest, Mine = nextMine }; // 실패 시 재료만 소모
        return isForestT3Target
            ? state with { Forest = nextForest with { Tier3 = nextForest.Tier3 + 1 }, Mine = nextMine }
            : state with { Forest = nextForest, Mine = nextMine with { Tier3 = nextMine.Tier3 + 1 } };
    }
}