using System.Collections.Generic;
using NUnit.Framework;

public class CraftingUnlockTests
{
    Dictionary<string, int> 레시피_해금_테이블() => new()
    {
        { "반죽", 1 },
        { "고급 반죽", 2 }
    };

    List<MixRecipe> 기본_레시피_목록() => new()
    {
        new(new[] { "물", "밀가루" }, "반죽"),
        new(new[] { "우유", "밀가루" }, "고급 반죽")
    };

    string? 제작_시도(int level, params string[] ingredients)
    {
        var available = RecipeFilter.GetAvailable(기본_레시피_목록(), level, 레시피_해금_테이블());
        return CraftingSystem.Mix(ingredients, available);
    }

    [Test]
    public void 요구_레벨보다_낮으면_제작되지_않고_null을_반환한다()
    {
        var result = 제작_시도(1, "우유", "밀가루");
        Assert.IsNull(result);
    }

    [Test]
    public void 요구_레벨을_만족하면_정상적으로_제작된다()
    {
        var result = 제작_시도(2, "우유", "밀가루");
        Assert.AreEqual("고급 반죽", result);
    }

    [Test]
    public void 해금_테이블에_없는_레시피는_1레벨부터_제작_가능하다()
    {
        var result = 제작_시도(1, "물", "밀가루");
        Assert.AreEqual("반죽", result);
    }
}