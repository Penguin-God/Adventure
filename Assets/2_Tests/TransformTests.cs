using System.Collections.Generic;
using NUnit.Framework;

public class TransformTests
{
    IEnumerable<TransformRecipe> CreateRecipes(params TransformRecipe[] customRecipes) =>
        customRecipes.Length > 0 ? customRecipes : new List<TransformRecipe>
        {
            new("빵", ProcessType.Bake, "빈즉"),
            new("빵", ProcessType.Ferment, "발효된 반죽")
        };

    void 변환_검증(string? expected, string ingredient, ProcessType processType)
    {
        Assert.AreEqual(expected, CraftingSystem.Transform(ingredient, processType, CreateRecipes()));
    }

    [Test]
    public void 변환_레시피대로_결과물이_나온다()
    {
        변환_검증("빈즉", "빵", ProcessType.Bake);
        변환_검증("발효된 반죽", "빵", ProcessType.Ferment);
        변환_검증(null, "돌", ProcessType.Bake);
    }
}