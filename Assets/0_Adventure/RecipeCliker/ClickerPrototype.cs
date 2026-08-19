namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record TerrainState(int Tier1, int Tier2, int Tier3);
public record ClickerState(TerrainState Forest, TerrainState Mine, int UltimateItem);

public static class ClickerLogic
{
    // --- 🌲 숲(Forest) 로직 ---
    public static ClickerState GatherForest(ClickerState state, float randomValue, float t2Chance, float t3Chance)
    {
        var nextForest = state.Forest;

        // 1. 가장 희귀한 Tier 3 당첨 여부부터 검사
        if (randomValue <= t3Chance)
        {
            nextForest = nextForest with { Tier3 = nextForest.Tier3 + 1 };
        }
        // 2. 그 다음 Tier 2 검사 (T3 확률 구간을 넘어선 값부터 T2 확률 구간까지)
        else if (randomValue <= t3Chance + t2Chance)
        {
            nextForest = nextForest with { Tier2 = nextForest.Tier2 + 1 };
        }
        // 3. 위 조건에 모두 빗나갔다면 기본 재료(Tier 1) 획득
        else
        {
            nextForest = nextForest with { Tier1 = nextForest.Tier1 + 1 };
        }

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

    // --- ⛰️ 광산(Mine) 로직 (숲과 구조 동일) ---
    public static ClickerState GatherMine(ClickerState state, float randomValue, float t2Chance, float t3Chance)
    {
        var nextMine = state.Mine;

        if (randomValue <= t3Chance)
            nextMine = nextMine with { Tier3 = nextMine.Tier3 + 1 };
        else if (randomValue <= t3Chance + t2Chance)
            nextMine = nextMine with { Tier2 = nextMine.Tier2 + 1 };
        else
            nextMine = nextMine with { Tier1 = nextMine.Tier1 + 1 };

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