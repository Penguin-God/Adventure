using System.Collections.Generic;
using System.Linq;

public record MixRecipe(IReadOnlyCollection<string> Ingredients, string Result);

public static class CraftingSystem
{
    public static string? Mix(IEnumerable<string> ingredients, IEnumerable<MixRecipe> mixRecipe)
    {
        return mixRecipe.FirstOrDefault(Equal)?.Result;
        bool Equal(MixRecipe recipe) => recipe.Ingredients.Count == ingredients.Count() && recipe.Ingredients.All(ingredients.Contains);
    }
}