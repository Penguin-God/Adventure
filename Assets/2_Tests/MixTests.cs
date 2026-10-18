using System.Collections.Generic;
using NUnit.Framework;

public class CraftingTests
{
    private CraftingSystem CreateSut(params CraftingRecipe[] customRecipes)
    {
        var defaultRecipes = new List<CraftingRecipe>
        {
            new(new[] { "Water", "Flour" }, "Dough")
        };

        IEnumerable<CraftingRecipe> recipes = customRecipes.Length > 0 ? customRecipes : defaultRecipes;
        return new CraftingSystem(recipes);
    }

    [Test]
    public void 물과_밀가루를_섞으면_반죽이_나온다()
    {
        var sut = CreateSut();
        var ingredients = new[] { "Water", "Flour" };

        var result = sut.Mix(ingredients);

        Assert.AreEqual("Dough", result);
    }

    [Test]
    public void 재료_순서가_바뀌어도_동일한_결과가_나온다()
    {
        var sut = CreateSut();
        var ingredients = new[] { "Flour", "Water" };

        var result = sut.Mix(ingredients);

        Assert.AreEqual("Dough", result);
    }

    [Test]
    public void 등록되지_않은_재료_조합이면_null을_반환한다()
    {
        var sut = CreateSut();
        var ingredients = new[] { "Water", "Stone" };

        var result = sut.Mix(ingredients);

        Assert.IsNull(result);
    }

    [Test]
    public void 재료_일부만_일치하면_조합되지_않는다()
    {
        var sut = CreateSut();
        var ingredients = new[] { "Water" };

        var result = sut.Mix(ingredients);

        Assert.IsNull(result);
    }
}