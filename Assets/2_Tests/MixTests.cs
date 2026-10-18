using System.Collections.Generic;
using NUnit.Framework;

public class CraftingTests
{
    IEnumerable<MixRecipe> CreateRecipes(params MixRecipe[] customRecipes) => new List<MixRecipe> { new(new[] { "물", "밀가루" }, "반죽") };

    void 재료_섞기_검증(string? excpted, params string[] ingredients)
    {
        Assert.AreEqual(excpted, CraftingSystem.Mix(ingredients, CreateRecipes()));
    }

    [Test]
    public void 순서_상관없이_레시피대로_섞으면_매치되는_결과가_나온다()
    {
        재료_섞기_검증("반죽", "물", "밀가루");
        재료_섞기_검증("반죽", "밀가루", "물");
    }

    [Test]
    public void 등록되지_않은_재료_조합이면_null을_반환한다()
    {
        재료_섞기_검증(null, "통밀", "물");
        재료_섞기_검증(null, "물");
    }
}