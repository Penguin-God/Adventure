using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record TerrainState(int Tier1, int Tier2, int Tier3);
public record ClickerState(TerrainState Forest, TerrainState Mine, int UltimateItem);

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
}

public static class ClickerLogic
{
    public static ClickerState UpdateForest(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Forest = updateFunc(state.Forest) };
    public static ClickerState UpdateMine(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Mine = updateFunc(state.Mine) };

    public static bool CanCraftUltimate(ClickerState state) => state.Forest.Tier3 >= 1 && state.Mine.Tier3 >= 1;

    public static ClickerState CraftUltimate(ClickerState state) => CanCraftUltimate(state) ?
        state with
        {
            Forest = state.Forest with { Tier3 = state.Forest.Tier3 - 1 },
            Mine = state.Mine with { Tier3 = state.Mine.Tier3 - 1 },
            UltimateItem = state.UltimateItem + 1
        }
        : state;
}