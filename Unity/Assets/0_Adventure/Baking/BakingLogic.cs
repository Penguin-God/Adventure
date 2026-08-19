using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 재료들을 하나로 섞어 새로운 혼합 재료(Ingredient)를 반환하는 순수 함수 클래스입니다.
/// </summary>
public static class Mixer
{
    public static Ingredient Mix(IEnumerable<Ingredient> items)
    {
        var allComponents = new List<Ingredient>();

        foreach (var item in items)
        {
            if (item.Components != null && item.Components.Count > 0)
                allComponents.AddRange(item.Components);
            else
                allComponents.Add(item);
        }

        var distinctComps = allComponents
            .GroupBy(c => c.Name + c.State)
            .Select(g => g.First())
            .ToArray();

        bool hasFlour = distinctComps.Any(c => c.Name == "밀가루");
        bool hasLiquid = distinctComps.Any(c => c.Name == "물" || c.Name == "계란");

        string newName = (hasFlour && hasLiquid) ? "반죽" : "혼합물";

        return new Ingredient(newName, "기본", distinctComps);
    }
}

/// <summary>
/// 재료의 상태(State)를 변화시키는 순수 함수 클래스입니다.
/// </summary>
public static class Processor
{
    public static Ingredient ApplyHeat(Ingredient item) =>
        item.Name switch
        {
            "물" => item with { State = "끓인" },
            "밀가루" => item with { State = "구운" },
            "버터" => item with { State = "헤이즐넛" },
            _ => item
        };

    public static Ingredient Ferment(Ingredient item)
    {
        if (item.Name == "반죽" && item.Components != null && item.Components.Any(c => c.Name == "이스트"))
        {
            return item with { State = "발효된" };
        }
        return item;
    }
}

/// <summary>
/// 반죽을 오븐에 넣어 레시피와 대조 후 결과물을 반환하는 클래스입니다.
/// </summary>
public static class Oven
{
    public static string Bake(Ingredient dough, IEnumerable<Recipe> recipes)
    {
        if (dough.Components == null || dough.Components.Count == 0)
            return "끔찍한 숯덩이 (단일 재료는 구울 수 없습니다)";

        var compNames = new HashSet<string>(
            dough.Components.Select(c => c.State == "기본" ? c.Name : $"{c.State} {c.Name}")
        );

        var matchedRecipe = recipes.FirstOrDefault(r =>
            r.RequiredState == dough.State &&
            compNames.SetEquals(r.RequiredComponentNames)
        );

        return matchedRecipe != null ? matchedRecipe.ResultName : "끔찍한 숯덩이 (알 수 없는 조합)";
    }
}