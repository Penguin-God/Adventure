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
    [SerializeField] List<MixRecipeData> mixRecipes = new();
    [SerializeField] List<TransformRecipeData> transformRecipes = new();

    public string? Mix(IEnumerable<string> ingredients)
    {
        var domainRecipes = mixRecipes.Select(r => r.ToDomain());
        var result = CraftingSystem.Mix(ingredients, domainRecipes);
        Debug.Log(result != null ? $"[CraftingManager] 섞기 성공: {result}" : "[CraftingManager] 섞기 실패 (일치하는 레시피 없음)");
        return result;
    }

    public string? Transform(string ingredient, ProcessType processType)
    {
        var domainRecipes = transformRecipes.Select(r => r.ToDomain());
        var result = CraftingSystem.Transform(ingredient, processType, domainRecipes);
        Debug.Log(result != null ? $"[CraftingManager] 변환 성공: {ingredient} -> {result} ({processType})" : $"[CraftingManager] 변환 실패: {ingredient} ({processType})");
        return result;
    }
}