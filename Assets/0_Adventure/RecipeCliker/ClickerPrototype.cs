using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record TerrainState(int Tier1, int Tier2, int Tier3);
// ✨ 업그레이드 상태를 담는 새로운 레코드
public record UpgradeState(int Tier2Prob, int Tier3Prob, int SellBonus);
// ✨ ClickerState에 UpgradeState 포함
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

    public static TerrainState CraftTier2(TerrainState state) => CanCraftTier2(state) ? state with { Tier1 = state.Tier1 - 100, Tier2 = state.Tier2 + 1 } : state;
    public static TerrainState CraftTier3(TerrainState state) => CanCraftTier3(state) ? state with { Tier2 = state.Tier2 - 100, Tier3 = state.Tier3 + 1 } : state;

    public static (TerrainState next, int earned) SellTier1(TerrainState state, int price) => state.Tier1 > 0 ? (state with { Tier1 = state.Tier1 - 1 }, price) : (state, 0);
    public static (TerrainState next, int earned) SellTier2(TerrainState state, int price) => state.Tier2 > 0 ? (state with { Tier2 = state.Tier2 - 1 }, price) : (state, 0);
    public static (TerrainState next, int earned) SellTier3(TerrainState state, int price) => state.Tier3 > 0 ? (state with { Tier3 = state.Tier3 - 1 }, price) : (state, 0);
}

public static class ClickerLogic
{
    public static ClickerState UpdateForest(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Forest = updateFunc(state.Forest) };
    public static ClickerState UpdateMine(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Mine = updateFunc(state.Mine) };

    public static ClickerState SellForest(ClickerState state, Func<TerrainState, (TerrainState next, int earned)> sellFunc) =>
        sellFunc(state.Forest) switch { var (n, e) => state with { Forest = n, Money = state.Money + e } };

    public static ClickerState SellMine(ClickerState state, Func<TerrainState, (TerrainState next, int earned)> sellFunc) =>
        sellFunc(state.Mine) switch { var (n, e) => state with { Mine = n, Money = state.Money + e } };

    public static bool CanCraftUltimate(ClickerState state) => state.Forest.Tier3 >= 1 && state.Mine.Tier3 >= 1;

    public static ClickerState CraftUltimate(ClickerState state) => CanCraftUltimate(state) ?
        state with { Forest = state.Forest with { Tier3 = state.Forest.Tier3 - 1 }, Mine = state.Mine with { Tier3 = state.Mine.Tier3 - 1 }, UltimateItem = state.UltimateItem + 1 } : state;

    public static ClickerState SellUltimate(ClickerState state, int price) => state.UltimateItem > 0 ? state with { UltimateItem = state.UltimateItem - 1, Money = state.Money + price } : state;
    public static ClickerState BuyUpgrade(ClickerState state, int cost, Func<UpgradeState, UpgradeState> upgradeFunc) => state.Money >= cost ? state with { Money = state.Money - cost, Upgrades = upgradeFunc(state.Upgrades) } : state;
}