namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record ClickerState(int Tier1, int Tier2, int Tier3);

public static class ClickerLogic
{
    // 무작위 값(randomValue)과 기준 확률(dropChance)을 받아 비교 로직을 직접 수행합니다.
    public static ClickerState Gather(ClickerState state, float randomValue, float dropChance)
    {
        var nextState = state with { Tier1 = state.Tier1 + 1 };

        if (randomValue <= dropChance)
            nextState = nextState with { Tier2 = nextState.Tier2 + 1 };

        return nextState;
    }

    public static ClickerState CraftTier2(ClickerState state)
    {
        if (state.Tier1 >= 100)
            return state with { Tier1 = state.Tier1 - 100, Tier2 = state.Tier2 + 1 };
        return state;
    }

    public static ClickerState CraftTier3(ClickerState state)
    {
        if (state.Tier2 >= 100)
            return state with { Tier2 = state.Tier2 - 100, Tier3 = state.Tier3 + 1 };
        return state;
    }
}