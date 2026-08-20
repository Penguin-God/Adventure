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

    public static TerrainState CraftTier2(TerrainState state) => state.Tier1 >= 100 ? state with { Tier1 = state.Tier1 - 100, Tier2 = state.Tier2 + 1 } : state;
    public static TerrainState CraftTier3(TerrainState state) => state.Tier2 >= 100 ? state with { Tier2 = state.Tier2 - 100, Tier3 = state.Tier3 + 1 } : state;
}

public static class ClickerLogic
{
    public static ClickerState UpdateForest(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Forest = updateFunc(state.Forest) };
    public static ClickerState UpdateMine(ClickerState state, Func<TerrainState, TerrainState> updateFunc) => state with { Mine = updateFunc(state.Mine) };

    // --- 궁극의 상위 조합 (이건 두 지형이 모두 필요하므로 ClickerState 레벨에서 처리) ---
    public static ClickerState CraftUltimate(ClickerState state) => (state.Forest.Tier3 >= 1 && state.Mine.Tier3 >= 1) ?
        state with
            {
                Forest = state.Forest with { Tier3 = state.Forest.Tier3 - 1 },
                Mine = state.Mine with { Tier3 = state.Mine.Tier3 - 1 },
                UltimateItem = state.UltimateItem + 1
            }
            : state;
}