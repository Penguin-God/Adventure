using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

/// <summary>
/// 단일 재료와 혼합 재료를 모두 아우르는 범용 레코드입니다.
/// Components가 비어있으면 단일 재료, 있으면 혼합 재료로 취급됩니다.
/// </summary>
public record Ingredient(string Name, string State = "기본", IReadOnlyCollection<Ingredient> Components = null)
{
    // 괄호 안에 구성 요소를 나열하여 보여주는 UI용 문자열 반환 함수
    public string GetDisplayName()
    {
        string baseStr = State == "기본" ? Name : $"{State} {Name}";
        if (Components != null && Components.Count > 0)
        {
            var compNames = Components.Select(c => c.State == "기본" ? c.Name : $"{c.State} {c.Name}");
            return $"{baseStr}({string.Join(", ", compNames)})";
        }
        return baseStr;
    }
}

/// <summary>
/// 재료들을 하나로 섞어 새로운 혼합 재료(Ingredient)를 반환하는 순수 함수 클래스입니다.
/// </summary>
public static class Mixer
{
    public static Ingredient Mix(IEnumerable<Ingredient> items)
    {
        var allComponents = new List<Ingredient>();

        // 기존에 섞여 있던 혼합물이라면 내부 구성 요소를 모두 펼쳐서(Flatten) 합칩니다.
        foreach (var item in items)
        {
            if (item.Components != null && item.Components.Count > 0)
                allComponents.AddRange(item.Components);
            else
                allComponents.Add(item);
        }

        // 중복된 구성 요소 제거 (예: 밀가루를 두 번 넣어도 하나로 취급)
        var distinctComps = allComponents
            .GroupBy(c => c.Name + c.State)
            .Select(g => g.First())
            .ToArray();

        bool hasFlour = distinctComps.Any(c => c.Name == "밀가루");
        bool hasWater = distinctComps.Any(c => c.Name == "물");

        // 밀가루와 물이 모두 포함되어 있으면 '반죽', 아니면 '혼합물'로 명명
        string newName = (hasFlour && hasWater) ? "반죽" : "혼합물";

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
            _ => item // 열을 가해도 변하지 않는 재료는 원본 반환
        };

    public static Ingredient Ferment(Ingredient item)
    {
        // 대상이 '반죽'이고, 구성 요소 중에 '이스트'가 포함되어 있을 때만 발효 상태로 변경
        if (item.Name == "반죽" && item.Components != null && item.Components.Any(c => c.Name == "이스트"))
        {
            return item with { State = "발효된" };
        }
        return item; // 조건이 안 맞으면 원본 반환
    }
}

/// <summary>
/// 완성품의 이름, 요구되는 반죽의 상태(State), 그리고 필수 구성 요소(이름)를 정의합니다.
/// </summary>
public record Recipe(string ResultName, string RequiredState, string[] RequiredComponentNames);

/// <summary>
/// 반죽을 오븐에 넣어 레시피와 대조 후 결과물을 반환하는 클래스입니다.
/// </summary>
public static class Oven
{
    public static string Bake(Ingredient dough, IEnumerable<Recipe> recipes)
    {
        if (dough.Components == null || dough.Components.Count == 0)
            return "끔찍한 숯덩이 (단일 재료는 구울 수 없습니다)";

        var compNames = new HashSet<string>(dough.Components.Select(c => c.Name));

        // 반죽의 상태(State)와 구성 요소(Components)가 정확히 일치하는 레시피 탐색
        var matchedRecipe = recipes.FirstOrDefault(r =>
            r.RequiredState == dough.State &&
            compNames.SetEquals(r.RequiredComponentNames)
        );

        return matchedRecipe != null ? matchedRecipe.ResultName : "끔찍한 숯덩이 (알 수 없는 조합)";
    }
}