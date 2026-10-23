using System.Collections.Generic;

public record LevelProgress(int Level, int CurrentExp);

public static class LevelSystem
{
    public static LevelProgress AddExp(LevelProgress current, string item, IReadOnlyDictionary<string, int> itemExpTable, IReadOnlyList<int> requiredExpTable)
    {
        int expToAdd = itemExpTable.TryGetValue(item, out var exp) ? exp : 1;
        return AddExp(current, expToAdd, requiredExpTable);
    }

    public static LevelProgress AddExp(LevelProgress current, int expToAdd, IReadOnlyList<int> requiredExpTable)
    {
        int requiredIndex = current.Level - 1;
        // 최대레벨
        if (requiredIndex >= requiredExpTable.Count) return new LevelProgress(current.Level, 0);

        int totalExp = current.CurrentExp + expToAdd;
        if (totalExp < requiredExpTable[requiredIndex]) return new LevelProgress(current.Level, totalExp);

        // 레벨업 후 재귀
        return AddExp(new LevelProgress(current.Level + 1, 0), totalExp - requiredExpTable[requiredIndex], requiredExpTable);
    }
}