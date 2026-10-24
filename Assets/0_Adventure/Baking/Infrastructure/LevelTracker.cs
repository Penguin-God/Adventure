using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;

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

public class LevelTracker : MonoBehaviour
{
    [SerializeField] List<int> requiredExpTable = new() { 100, 200, 300 };
    [SerializeField] List<ItemExpData> itemExpSettings = new();
    [field: SerializeField] public LevelProgress Progress { get; private set; } = new(1, 0);

    public void AddExpByItem(string item)
    {
        var expTable = itemExpSettings.ToDictionary(x => x.item, x => x.exp);
        var prevLevel = Progress.Level;

        Progress = LevelSystem.AddExp(Progress, item, expTable, requiredExpTable);

        Debug.Log($"[{gameObject.name}] '{item}' 처리 -> 레벨: {Progress.Level} (Exp: {Progress.CurrentExp})");
        if (Progress.Level > prevLevel) Debug.Log($"[{gameObject.name}] 레벨업! ({prevLevel} -> {Progress.Level})");
    }
}