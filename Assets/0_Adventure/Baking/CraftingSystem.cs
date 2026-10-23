using System.Collections.Generic;
using System.Linq;

public static class CraftingSystem
{
    public static string? Mix(IEnumerable<string> ingredients, IEnumerable<MixRecipe> mixRecipe)
    {
        return mixRecipe.FirstOrDefault(Equal)?.Result;
        bool Equal(MixRecipe recipe) => recipe.Ingredients.Count == ingredients.Count() && recipe.Ingredients.All(ingredients.Contains);
    }

    public static string? Transform(string ingredient, ProcessType processType, IEnumerable<TransformRecipe> recipes) 
        => recipes.FirstOrDefault(recipe => recipe.Ingredient == ingredient && recipe.ProcessType == processType)?.Result;
}

public static class RecipeFilter
{
    public static IEnumerable<MixRecipe> GetAvailable(IEnumerable<MixRecipe> recipes, int currentLevel, IReadOnlyDictionary<string, int> unlockLevels)
        => recipes.Where(r => currentLevel >= (unlockLevels.TryGetValue(r.Result, out var req) ? req : 1));

    public static IEnumerable<TransformRecipe> GetAvailable(IEnumerable<TransformRecipe> recipes, int currentLevel, IReadOnlyDictionary<string, int> unlockLevels)
        => recipes.Where(r => currentLevel >= (unlockLevels.TryGetValue(r.Result, out var req) ? req : 1));
}