using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

// [데이터 모델] 가치를 나타내는 Price 속성 추가
public record Ingredient(string Name, string State = "Raw", IReadOnlyCollection<Ingredient> Components = null, int Price = 0)
{
    public string GetDisplayName()
    {
        string baseStr = State == "Raw" ? Name : $"{State} {Name}";
        if (Components != null && Components.Count > 0)
        {
            var compNames = Components.Select(c => c.State == "Raw" ? c.Name : $"{c.State} {c.Name}");
            return $"{baseStr}({string.Join(", ", compNames)})";
        }
        return baseStr;
    }
}

public record Recipe(string ResultName, string RequiredState, string[] RequiredComponentNames, int Price);

// [혼합 로직]
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
        // 액체류(물, 계란 등)가 포함되어 있는지 검사
        bool hasLiquid = distinctComps.Any(c => c.Name == "물" || c.Name == "계란");

        string newName = (hasFlour && hasLiquid) ? "반죽" : "혼합재료";
        return new Ingredient(newName, "Raw", distinctComps);
    }
}

// [가공 로직]
public static class Processor
{
    public static Ingredient ApplyHeat(Ingredient item)
    {
        // 🌟 버터 -> 불 -> 헤이즐넛 버터
        if (item.Name == "버터") return item with { Name = "헤이즐넛 버터" };

        return item.Name switch
        {
            "물" => item with { State = "끓인" },
            "밀가루" => item with { State = "구운" },
            _ => item
        };
    }

    public static Ingredient Ferment(Ingredient item)
    {
        // 🌟 이스트가 있는 반죽 -> 발효
        if (item.Name == "반죽" && item.Components != null && item.Components.Any(c => c.Name == "이스트"))
        {
            return item with { State = "발효된" };
        }
        return item;
    }
}

// [오븐 로직]
public static class Oven
{
    public static Ingredient Bake(Ingredient dough, IEnumerable<Recipe> recipes)
    {
        if (dough.Components == null || dough.Components.Count == 0)
            return new Ingredient("숯덩이", "실패", null, 0);

        var compNames = new HashSet<string>(
            dough.Components.Select(c => c.State == "Raw" ? c.Name : $"{c.State} {c.Name}")
        );

        // 상태와 구성 요소가 완벽히 일치하는 레시피 탐색
        var matchedRecipe = recipes.FirstOrDefault(r =>
            r.RequiredState == dough.State &&
            compNames.SetEquals(r.RequiredComponentNames)
        );

        if (matchedRecipe != null)
        {
            // 성공 시 가격이 포함된 완성품 반환
            return new Ingredient(matchedRecipe.ResultName, "완성품", null, matchedRecipe.Price);
        }

        return new Ingredient("괴식 덩어리", "실패", null, 0);
    }
}