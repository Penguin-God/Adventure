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