using System;
using System.Collections.Generic;
using System.Linq;

public record CraftingRecipe(IReadOnlyCollection<string> Ingredients, string Result);

public sealed class CraftingSystem
{
    private readonly IReadOnlyList<CraftingRecipe> _recipes;

    public CraftingSystem(IEnumerable<CraftingRecipe> recipes)
    {
        _recipes = recipes?.ToList() ?? throw new ArgumentNullException(nameof(recipes));
    }

    public string? Mix(IEnumerable<string> ingredients)
    {
        if (ingredients == null)
        {
            return null;
        }

        var inputSet = new HashSet<string>(ingredients);

        return _recipes
            .FirstOrDefault(recipe => recipe.Ingredients.Count == inputSet.Count && recipe.Ingredients.All(inputSet.Contains))
            ?.Result;
    }
}