using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BakingPrototype : MonoBehaviour
{
    private List<Ingredient> inventory = new List<Ingredient>();
    private List<object> workbench = new List<object>();
    private string logMessage = "재료를 선택해 작업대에 올리세요!";

    private readonly List<Recipe> recipes = new List<Recipe>
    {
        new Recipe("[대성공] 겉바속촉 완벽한 빵!", new[]
        {
            new Ingredient("밀가루", "기본"),
            new Ingredient("물", "기본"),
            new Ingredient("이스트", "발효됨")
        }),
        new Recipe("[성공] 딱딱하고 질긴 빵", new[]
        {
            new Ingredient("밀가루", "기본"),
            new Ingredient("물", "기본")
        })
    };

    void Start()
    {
        RefillInventory();
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));
        GUILayout.Label($"[상태창] {logMessage}", GUILayout.Height(40));
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        DrawInventorySection();
        DrawWorkbenchSection();
        DrawActionSection();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void DrawInventorySection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("📦 창고");
        GUILayout.Space(10);

        for (int i = 0; i < inventory.Count; i++)
        {
            int index = i;
            var item = inventory[index];

            DrawItemRow($"[{item.State}] {item.Name}", "올리기 ➔", () =>
            {
                inventory.RemoveAt(index);
                workbench.Add(item);
                logMessage = $"{item.Name}을(를) 작업대에 올렸습니다.";
            });
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("➕ 재료 리필", GUILayout.Height(40)))
        {
            RefillInventory();
            logMessage = "창고에 재료가 리필되었습니다.";
        }
        GUILayout.EndVertical();
    }

    void DrawWorkbenchSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🛠️ 작업대");
        GUILayout.Space(10);

        for (int i = 0; i < workbench.Count; i++)
        {
            int index = i;
            var item = workbench[index];
            string displayName = GetDisplayName(item);

            DrawItemRow(displayName, "✖ 취소", () =>
            {
                workbench.RemoveAt(index);
                ReturnToInventory(item);
                logMessage = "작업대에서 항목을 창고로 되돌렸습니다.";
            });
        }
        GUILayout.EndVertical();
    }

    void DrawActionSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🔥 가공 및 완성");
        GUILayout.Space(10);

        if (workbench.Count == 1 && workbench[0] is Ingredient)
            DrawSingleProcessing();
        else if (workbench.Count > 1)
            DrawMixing();

        GUILayout.FlexibleSpace();
        if (workbench.Count > 0)
            DrawOven();

        GUILayout.EndVertical();
    }

    void DrawItemRow(string itemName, string buttonText, Action onClick)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(itemName);
        if (GUILayout.Button(buttonText, GUILayout.Width(100), GUILayout.Height(50)))
        {
            onClick?.Invoke();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    void DrawSingleProcessing()
    {
        var target = (Ingredient)workbench[0];

        if (GUILayout.Button("🔥 불 가열", GUILayout.Height(60)))
        {
            var processed = Processor.ApplyHeat(target);
            if (processed != target)
            {
                workbench[0] = processed;
                logMessage = $"{processed.Name}에 불을 가해 [{processed.State}] 상태가 되었습니다.";
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("🦠 발효", GUILayout.Height(60)))
        {
            var processed = Processor.Ferment(target);
            if (processed != target)
            {
                workbench[0] = processed;
                logMessage = $"{processed.Name}을(를) 발효시켰습니다.";
            }
        }
    }

    void DrawMixing()
    {
        if (GUILayout.Button("🥣 재료 섞기", GUILayout.Height(80)))
        {
            var ingredients = workbench.OfType<Ingredient>().ToArray();
            var doughs = workbench.OfType<Dough>().ToArray();

            if (ingredients.Length == workbench.Count)
            {
                UpdateWorkbench(Dough.Mix(ingredients), "재료들을 섞어 [반죽]을 만들었습니다!");
            }
            else if (doughs.Length == 1 && ingredients.Length > 0)
            {
                var newDough = doughs[0];
                foreach (var ing in ingredients)
                {
                    newDough = Dough.Mix(newDough, ing);
                }
                UpdateWorkbench(newDough, "기존 반죽에 새로운 재료를 섞어 넣었습니다!");
            }
            else
            {
                logMessage = "섞을 수 없는 조합입니다 (반죽끼리 섞는 것은 미지원).";
            }
        }
    }

    void DrawOven()
    {
        if (GUILayout.Button("🔥 오븐에 굽기", GUILayout.Height(80)))
        {
            if (workbench.Count == 1 && workbench[0] is Dough dough)
            {
                string result = Oven.Bake(dough, recipes);
                workbench.Clear();
                logMessage = $"결과: {result}";
            }
            else
            {
                logMessage = "[실패] 오븐에는 완성된 '반죽' 1개만 넣을 수 있습니다.";
            }
        }
    }

    void UpdateWorkbench(object newItem, string message)
    {
        workbench.Clear();
        workbench.Add(newItem);
        logMessage = message;
    }

    string GetDisplayName(object item)
    {
        return item switch
        {
            Ingredient ing => $"[{ing.State}] {ing.Name}",
            Dough d => $"[반죽] 재료 {d.Components.Count}개",
            _ => "알 수 없는 물질"
        };
    }

    void ReturnToInventory(object item)
    {
        if (item is Ingredient ing)
        {
            inventory.Add(ing);
        }
        else if (item is Dough dough)
        {
            foreach (var component in dough.Components)
            {
                inventory.Add(component);
            }
        }
    }

    void RefillInventory()
    {
        inventory.Clear();
        inventory.Add(new Ingredient("밀가루"));
        inventory.Add(new Ingredient("물"));
        inventory.Add(new Ingredient("이스트"));
    }
}

public static class Processor
{
    public static Ingredient ApplyHeat(Ingredient item) =>
        item.Name switch
        {
            "물" => item with { State = "끓임" },
            "밀가루" => item with { State = "구움" },
            _ => item
        };

    public static Ingredient Ferment(Ingredient item) => item.Name == "이스트" ? item with { State = "발효됨" } : item;
}

public static class Oven
{
    public static string Bake(Dough dough, IEnumerable<Recipe> recipes)
    {
        var doughSet = new HashSet<Ingredient>(dough.Components);
        var matchedRecipe = recipes.FirstOrDefault(r => doughSet.SetEquals(r.RequiredComponents));

        return matchedRecipe != null ? matchedRecipe.ResultName : "끔찍한 숯덩이 (조합 실패)";
    }
}