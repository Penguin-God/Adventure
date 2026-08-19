using System.Collections.Generic;
using UnityEngine;

public class BakingPrototype : MonoBehaviour
{
    private List<Ingredient> inventory = new List<Ingredient>();
    private List<Ingredient> workbench = new List<Ingredient>();
    private string logMessage = "재료를 조합해 빵을 만들고 판매하세요!";

    // 🌟 판매 수익 관리
    private int currentMoney = 0;

    // 🌟 기획하신 3가지 레시피 세팅
    private readonly List<Recipe> recipes = new List<Recipe>
    {
        new Recipe("빵", "발효된", new[] { "밀가루", "물", "이스트" }, 40),
        new Recipe("파운드 케이크", "Raw", new[] { "밀가루", "계란", "버터", "설탕" }, 80),
        new Recipe("휘낭시에", "Raw", new[] { "밀가루", "계란", "헤이즐넛 버터", "설탕" }, 120)
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

        // 상단 재화 표시 UI
        GUILayout.BeginHorizontal();
        GUILayout.Label($"[상태창] {logMessage}", GUILayout.Height(40));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {currentMoney} G", GUILayout.Height(40));
        GUILayout.EndHorizontal();
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

            GUILayout.BeginHorizontal();
            GUILayout.Label(item.GetDisplayName());

            // 🌟 가격이 부여된 완성품은 작업대에 올리지 않고 바로 판매
            if (item.Price > 0)
            {
                if (GUILayout.Button($"💰 판매 (+{item.Price}G)", GUILayout.Width(130), GUILayout.Height(50)))
                {
                    currentMoney += item.Price;
                    inventory.RemoveAt(index);
                    logMessage = $"{item.Name}을(를) 팔아 {item.Price}G를 벌었습니다!";
                }
            }
            else
            {
                if (GUILayout.Button("올리기 ➔", GUILayout.Width(100), GUILayout.Height(50)))
                {
                    inventory.RemoveAt(index);
                    workbench.Add(item);
                    logMessage = $"{item.Name}을(를) 작업대에 올렸습니다.";
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("➕ 재료 리필", GUILayout.Height(40)))
        {
            RefillInventory();
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

            GUILayout.BeginHorizontal();
            GUILayout.Label(item.GetDisplayName());
            if (GUILayout.Button("✖ 취소", GUILayout.Width(80), GUILayout.Height(50)))
            {
                workbench.RemoveAt(index);
                ReturnToInventory(item);
                logMessage = "재료를 창고로 되돌렸습니다.";
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();
    }

    void DrawActionSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🔥 가공 및 완성");
        GUILayout.Space(10);

        if (workbench.Count == 1) DrawSingleProcessing();
        else if (workbench.Count > 1) DrawMixing();

        GUILayout.FlexibleSpace();
        if (workbench.Count > 0) DrawOven();

        GUILayout.EndVertical();
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
                logMessage = $"{target.Name} -> {processed.Name} ({processed.State})";
            }
            else logMessage = "불을 가해도 변화가 없습니다.";
        }
        GUILayout.Space(10);
        if (GUILayout.Button("🦠 발효", GUILayout.Height(60)))
        {
            var processed = Processor.Ferment(target);
            if (processed != target)
            {
                workbench[0] = processed;
                logMessage = $"{processed.GetDisplayName()}이(가) 되었습니다!";
            }
            else logMessage = "발효할 수 없는 상태입니다.";
        }
    }

    void DrawMixing()
    {
        if (GUILayout.Button("🥣 재료 섞기", GUILayout.Height(80)))
        {
            var mixedIngredient = Mixer.Mix(workbench);
            workbench.Clear();
            workbench.Add(mixedIngredient);
            logMessage = $"재료를 섞어 {mixedIngredient.GetDisplayName()}을(를) 만들었습니다!";
        }
    }

    void DrawOven()
    {
        if (GUILayout.Button("🔥 오븐에 굽기", GUILayout.Height(80)))
        {
            if (workbench.Count == 1)
            {
                // 🌟 오븐 결과물이 이제 Ingredient 객체로 반환됩니다.
                Ingredient result = Oven.Bake(workbench[0], recipes);
                workbench.Clear();

                if (result.Price > 0)
                {
                    inventory.Add(result); // 완성품을 창고로 옮겨 판매 대기
                    logMessage = $"[대성공] {result.Name} 완성! 창고에서 판매하세요.";
                }
                else
                {
                    logMessage = $"[실패] {result.Name}이(가) 되었습니다...";
                }
            }
            else
            {
                logMessage = "오븐에는 반죽(혼합재료) 1개만 넣을 수 있습니다.";
            }
        }
    }

    void ReturnToInventory(Ingredient item)
    {
        if (item.Components != null && item.Components.Count > 0)
        {
            foreach (var component in item.Components) inventory.Add(component);
        }
        else inventory.Add(item);
    }

    void RefillInventory()
    {
        inventory.Clear();
        string[] baseItems = { "밀가루", "물", "이스트", "계란", "버터", "설탕" };
        foreach (var item in baseItems)
        {
            inventory.Add(new Ingredient(item));
        }
    }
}