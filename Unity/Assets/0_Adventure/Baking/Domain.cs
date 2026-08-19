using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    // 유니티(구버전 C#)에서 record 키워드를 사용하기 위한 필수 설정입니다.
    internal static class IsExternalInit { }
}

// 단일 재료와 혼합 재료를 모두 아우르는 범용 레코드입니다.
public record Ingredient(string Name, string State = "기본", IReadOnlyCollection<Ingredient> Components = null)
{
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

public record Recipe(string ResultName, string RequiredState, string[] RequiredComponentNames);