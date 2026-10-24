using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ItemExpData
{
    public string item;
    public int exp;
}

[Serializable]
public struct RecipeUnlockData
{
    public string resultItem;
    public int unlockLevel;
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<int> requiredExpTable;
    [SerializeField] List<ItemExpData> itemExpSettings = new();
    [field: SerializeField] public LevelProgress Progress { get; private set; } = new(1, 0);

    public void AddExpByItem(string item)
    {
        var expTable = itemExpSettings.ToDictionary(x => x.item, x => x.exp);
        var prevLevel = Progress.Level;

        Progress = LevelSystem.AddExp(Progress, item, expTable, requiredExpTable);

        Debug.Log($"[LevelManager] '{item}' 처리 -> 레벨: {Progress.Level}, 경험치: {Progress.CurrentExp}");
        if (Progress.Level > prevLevel) Debug.Log($"[LevelManager] 레벨업! ({prevLevel} -> {Progress.Level})");
    }
}