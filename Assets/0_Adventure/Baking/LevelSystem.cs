using System.Collections.Generic;

public record LevelProgress(int Level, int CurrentExp);

public static class LevelSystem
{
    public static LevelProgress AddExp(LevelProgress current, int expToAdd, IReadOnlyList<int> requiredExpTable)
    {
        int needExpIndex = current.Level - 1;
        // 최대레벨
        if (needExpIndex >= requiredExpTable.Count) return new LevelProgress(current.Level, 0);

        int totalExp = current.CurrentExp + expToAdd;
        if (totalExp < requiredExpTable[needExpIndex]) return new LevelProgress(current.Level, totalExp);

        // 렙업 후 다시 계산
        return AddExp(new LevelProgress(current.Level + 1, 0), totalExp - requiredExpTable[needExpIndex], requiredExpTable);
    }
}