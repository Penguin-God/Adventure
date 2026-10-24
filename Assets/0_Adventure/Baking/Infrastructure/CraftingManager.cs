using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public struct MixRecipeData
{
    public List<string> ingredients;
    public string result;

    public MixRecipe ToDomain() => new(ingredients, result);
}

[Serializable]
public struct TransformRecipeData
{
    public string ingredient;
    public ProcessType processType;
    public string result;

    public TransformRecipe ToDomain() => new(ingredient, processType, result);
}

public class CraftingManager : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    [SerializeField] List<MixRecipeData> mixRecipes = new();
    [SerializeField] List<TransformRecipeData> transformRecipes = new();
    [SerializeField] List<RecipeUnlockData> unlockSettings = new();

    public string? Mix(IEnumerable<string> ingredients)
    {
        var unlockTable = unlockSettings.ToDictionary(x => x.resultItem, x => x.unlockLevel);
        var domainRecipes = mixRecipes.Select(r => r.ToDomain());
        var availableRecipes = RecipeFilter.GetAvailable(domainRecipes, levelManager.Progress.Level, unlockTable);

        var result = CraftingSystem.Mix(ingredients, availableRecipes);
        Debug.Log(result != null ? $"[CraftingManager] 섞기 성공: {result}" : "[CraftingManager] 섞기 실패 (레시피 없음 또는 레벨 미달)");
        return result;
    }

    public string? Transform(string ingredient, ProcessType processType)
    {
        var unlockTable = unlockSettings.ToDictionary(x => x.resultItem, x => x.unlockLevel);
        var domainRecipes = transformRecipes.Select(r => r.ToDomain());
        var availableRecipes = RecipeFilter.GetAvailable(domainRecipes, levelManager.Progress.Level, unlockTable);

        var result = CraftingSystem.Transform(ingredient, processType, availableRecipes);
        Debug.Log(result != null ? $"[CraftingManager] 변환 성공: {ingredient} -> {result}" : $"[CraftingManager] 변환 실패 (레시피 없음 또는 레벨 미달)");
        return result;
    }
}