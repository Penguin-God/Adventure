using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Ingredient(string Name, string State = "Raw");

public record Dough(IReadOnlyCollection<Ingredient> Components)
{
    public static Dough Mix(params Ingredient[] ingredients) => new Dough(new HashSet<Ingredient>(ingredients));

    public static Dough Mix(Dough existingDough, Ingredient newIngredient)
    {
        var newSet = new HashSet<Ingredient>(existingDough.Components) { newIngredient };
        return new Dough(newSet);
    }
}

public record Recipe(string ResultName, IReadOnlyCollection<Ingredient> RequiredComponents);