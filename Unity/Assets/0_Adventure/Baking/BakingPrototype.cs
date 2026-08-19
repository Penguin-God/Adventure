using System;
using System.Collections.Generic;
using UnityEngine;

public class BakingPrototype : MonoBehaviour
{
    private List<Ingredient> inventory = new List<Ingredient>();
    private List<Ingredient> workbench = new List<Ingredient>(); // 이제 단일 타입으로 완벽 통일!
    private string logMessage = "재료를 선택해 작업대에 올리세요!";

    private readonly List<Recipe> recipes = new List<Recipe>
    {
        new Recipe("[대성공] 겉바속촉 완벽한 빵!", "발효된", new[] { "밀가루", "물", "이스트" }),
        new Recipe("[성공] 딱딱하고 질긴 빵", "기본", new[] { "밀가루", "물" })
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

            DrawItemRow(item.GetDisplayName(), "올리기 ➔", () =>
            {
                inventory.RemoveAt(index);
                workbench.Add(item);
                logMessage = $"{item.GetDisplayName()}을(를) 작업대에 올렸습니다.";
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

            DrawItemRow(item.GetDisplayName(), "✖ 취소", () =>
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

        if (workbench.Count == 1)
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
        var target = workbench[0];

        if (GUILayout.Button("🔥 불 가열", GUILayout.Height(60)))
        {
            var processed = Processor.ApplyHeat(target);
            if (processed != target)
            {
                workbench[0] = processed;
                logMessage = $"{target.Name}에 불을 가해 [{processed.State}] 상태가 되었습니다.";
            }
            else
            {
                logMessage = "이 재료는 열을 가해도 아무 반응이 없습니다.";
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("🦠 발효", GUILayout.Height(60)))
        {
            var processed = Processor.Ferment(target);
            if (processed != target)
            {
                workbench[0] = processed;
                logMessage = $"{processed.GetDisplayName()} 상태가 되었습니다!";
            }
            else if (target.Name != "반죽")
            {
                logMessage = "단일 재료는 발효할 수 없습니다. 반죽을 만들어주세요.";
            }
            else
            {
                logMessage = "반죽에 이스트가 포함되어 있지 않아 발효되지 않습니다.";
            }
        }
    }

    void DrawMixing()
    {
        if (GUILayout.Button("🥣 재료 섞기", GUILayout.Height(80)))
        {
            var mixedIngredient = Mixer.Mix(workbench);
            workbench.Clear();
            workbench.Add(mixedIngredient);
            logMessage = $"재료들을 섞어 {mixedIngredient.GetDisplayName()}을(를) 만들었습니다!";
        }
    }

    void DrawOven()
    {
        if (GUILayout.Button("🔥 오븐에 굽기", GUILayout.Height(80)))
        {
            if (workbench.Count == 1)
            {
                string result = Oven.Bake(workbench[0], recipes);
                workbench.Clear();
                logMessage = $"[오븐 결과] {result}";
            }
            else
            {
                logMessage = "오븐에는 가공이 끝난 재료 1개만 넣을 수 있습니다.";
            }
        }
    }

    void ReturnToInventory(Ingredient item)
    {
        if (item.Components != null && item.Components.Count > 0)
        {
            // 혼합물 취소 시 구성 요소들을 분해해서 창고로 돌려보냄
            foreach (var component in item.Components)
            {
                inventory.Add(component);
            }
        }
        else
        {
            inventory.Add(item);
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