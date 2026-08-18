using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Ingredient(string Name, string State = "Raw");

// [반죽] - IReadOnlyCollection으로 외부 변조를 막음
public record Dough(IReadOnlyCollection<Ingredient> Components)
{
    // 순수 함수: 재료 배열을 받아 '새로운' HashSet을 생성하여 캡슐화
    public static Dough Mix(params Ingredient[] ingredients) =>
        new Dough(new HashSet<Ingredient>(ingredients));

    // 순수 함수: Copy-on-Write 기법. 기존 컬렉션을 복사한 뒤 재료를 추가하여 반환
    public static Dough Mix(Dough existingDough, Ingredient newIngredient)
    {
        // 원본(existingDough.Components)은 전혀 훼손되지 않습니다.
        var newSet = new HashSet<Ingredient>(existingDough.Components)
            {
                newIngredient
            };
        return new Dough(newSet);
    }
}

// [레시피] - 결과물과 필요한 재료의 집합
public record Recipe(string ResultName, IReadOnlyCollection<Ingredient> RequiredComponents);


public static class Processor
{
    // 입력값을 받아 상태가 변한 '새로운' 객체를 반환 (원본 유지)
    public static Ingredient ApplyHeat(Ingredient item) =>
        item.Name switch
        {
            "Water" => item with { State = "Boiled" },
            "Flour" => item with { State = "Roasted" },
            "Dough" => item with { State = "Baked" },
            _ => item // 변화가 없는 재료는 원본 그대로 반환
        };

    public static Ingredient Ferment(Ingredient item) =>
        item.Name == "Yeast" ? item with { State = "Fermented" } : item;
}

// [오븐]
public static class Oven
{
    // 반죽과 레시피 목록을 받아 일치 여부를 검사하고 문자열(결과)을 반환하는 순수 함수
    public static string Bake(Dough dough, IEnumerable<Recipe> recipes)
    {
        // 순서 상관없이 재료의 구성만 비교하기 위해 HashSet의 SetEquals 활용
        var doughSet = new HashSet<Ingredient>(dough.Components);

        // 매칭되는 첫 번째 레시피 탐색
        var matchedRecipe = recipes.FirstOrDefault(r =>
            doughSet.SetEquals(r.RequiredComponents));

        // 레시피가 없으면 실패 문자열 반환
        return matchedRecipe != null ? matchedRecipe.ResultName : "Failure (Ruined Dough)";
    }
}