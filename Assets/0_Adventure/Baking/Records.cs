using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum ProcessType
{
    Bake,
    Ferment
}

public record MixRecipe(IReadOnlyCollection<string> Ingredients, string Result);
public record TransformRecipe(string Ingredient, ProcessType ProcessType, string Result);