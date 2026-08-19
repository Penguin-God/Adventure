namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}


public record TerrainState(int Tier1, int Tier2, int Tier3);
public record ClickerState(TerrainState Forest, TerrainState Mine, int UltimateItem);

public static class ClickerLogic
{
    // --- 🌲 숲(Forest) 로직 ---
    public static ClickerState GatherForest(ClickerState state, float randomValue, float dropChance)
    {
        // Forest 상태만 업데이트된 새로운 복사본을 만듭니다.
        var nextForest = state.Forest with { Tier1 = state.Forest.Tier1 + 1 };

        if (randomValue <= dropChance)
            nextForest = nextForest with { Tier2 = nextForest.Tier2 + 1 };

        // 전체 상태 중 Forest만 교체하여 반환합니다.
        return state with { Forest = nextForest };
    }

    public static ClickerState CraftForestTier2(ClickerState state)
    {
        if (state.Forest.Tier1 >= 100)
            return state with { Forest = state.Forest with { Tier1 = state.Forest.Tier1 - 100, Tier2 = state.Forest.Tier2 + 1 } };
        return state;
    }

    public static ClickerState CraftForestTier3(ClickerState state)
    {
        if (state.Forest.Tier2 >= 100)
            return state with { Forest = state.Forest with { Tier2 = state.Forest.Tier2 - 100, Tier3 = state.Forest.Tier3 + 1 } };
        return state;
    }

    // --- ⛰️ 광산(Mine) 로직 ---
    public static ClickerState GatherMine(ClickerState state, float randomValue, float dropChance)
    {
        var nextMine = state.Mine with { Tier1 = state.Mine.Tier1 + 1 };

        if (randomValue <= dropChance)
            nextMine = nextMine with { Tier2 = nextMine.Tier2 + 1 };

        return state with { Mine = nextMine };
    }

    public static ClickerState CraftMineTier2(ClickerState state)
    {
        if (state.Mine.Tier1 >= 100)
            return state with { Mine = state.Mine with { Tier1 = state.Mine.Tier1 - 100, Tier2 = state.Mine.Tier2 + 1 } };
        return state;
    }

    public static ClickerState CraftMineTier3(ClickerState state)
    {
        if (state.Mine.Tier2 >= 100)
            return state with { Mine = state.Mine with { Tier2 = state.Mine.Tier2 - 100, Tier3 = state.Mine.Tier3 + 1 } };
        return state;
    }

    // --- ✨ 궁극의 상위 조합 레시피 ---
    public static ClickerState CraftUltimate(ClickerState state)
    {
        // 조건: 숲의 3티어 재료 1개 + 광산의 3티어 재료 1개
        if (state.Forest.Tier3 >= 1 && state.Mine.Tier3 >= 1)
        {
            return state with
            {
                Forest = state.Forest with { Tier3 = state.Forest.Tier3 - 1 },
                Mine = state.Mine with { Tier3 = state.Mine.Tier3 - 1 },
                UltimateItem = state.UltimateItem + 1
            };
        }
        return state;
    }
}