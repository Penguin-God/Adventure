using System.Collections.Generic;
using UnityEngine;

public class BakingPrototype : MonoBehaviour
{
    private List<Ingredient> inventory = new List<Ingredient>();
    private List<Ingredient> workbench = new List<Ingredient>();
    private string logMessage = "재료를 조합해 빵을 만들고 돈을 벌어보세요!";

    // 초기 자본금 (테스트를 위해 100G 제공)
    private int currentMoney = 100;

    // 🌟 브라우니 레시피 추가
    private readonly List<Recipe> recipes = new List<Recipe>
    {
        new Recipe("빵", "발효된", new[] { "밀가루", "물", "이스트" }, 40),
        new Recipe("파운드 케이크", "Raw", new[] { "밀가루", "계란", "버터", "설탕" }, 80),
        new Recipe("휘낭시에", "Raw", new[] { "밀가루", "계란", "헤이즐넛 버터", "설탕" }, 120),
        new Recipe("브라우니", "Raw", new[] { "밀가루", "계란", "버터", "설탕", "초콜릿" }, 250) // 🍫 신규 브라우니!
    };

    void Start()
    {
        RefillInventory();
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 18;
        GUI.skin.button.fontSize = 18;

        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));

        // 상태창 및 소지금
        GUILayout.BeginHorizontal();
        GUILayout.Label($"[상태창] {logMessage}", GUILayout.Height(40));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"💰 소지금: {currentMoney} G", GUILayout.Height(40));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // 4개의 구역으로 분할 렌더링
        GUILayout.BeginHorizontal();
        DrawInventorySection();
        DrawWorkbenchSection();
        DrawActionSection();
        DrawShopSection(); // 🌟 상점 구역 추가
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    void DrawInventorySection()
    {
        // 4분할을 위해 넓이 조정
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4f - 20));
        GUILayout.Label("📦 창고");
        GUILayout.Space(10);

        for (int i = 0; i < inventory.Count; i++)
        {
            int index = i;
            var item = inventory[index];

            GUILayout.BeginHorizontal();
            GUILayout.Label(item.GetDisplayName());

            // 🌟 1. 완성품인 경우 (판매)
            if (item.Price > 0)
            {
                if (GUILayout.Button($"💰 판매 (+{item.Price}G)", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    currentMoney += item.Price;
                    inventory.RemoveAt(index);
                    logMessage = $"{item.Name}을(를) 팔아 {item.Price}G를 벌었습니다!";
                }
            }
            // 🌟 2. 레시피 아이템인 경우 (읽기)
            else if (item.State == "Recipe")
            {
                if (GUILayout.Button("📖 읽기", GUILayout.Width(80), GUILayout.Height(40)))
                {
                    // 클릭 시 텍스트(힌트)만 출력하고 소모되지는 않음
                    logMessage = $"[{item.Name}] 초콜릿, 버터, 설탕, 계란, 밀가루를 섞어 오븐에 구우면 완성!";
                }
            }
            // 🌟 3. 일반 재료인 경우 (작업대에 올리기)
            else
            {
                if (GUILayout.Button("올리기 ➔", GUILayout.Width(80), GUILayout.Height(40)))
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
        if (GUILayout.Button("➕ 기본 재료 리필", GUILayout.Height(40))) RefillInventory();
        GUILayout.EndVertical();
    }

    void DrawWorkbenchSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4f - 20));
        GUILayout.Label("🛠️ 작업대");
        GUILayout.Space(10);

        for (int i = 0; i < workbench.Count; i++)
        {
            int index = i;
            var item = workbench[index];
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.GetDisplayName());
            if (GUILayout.Button("✖ 취소", GUILayout.Width(60), GUILayout.Height(40)))
            {
                workbench.RemoveAt(index);
                ReturnToInventory(item);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();
    }

    void DrawActionSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4f - 20));
        GUILayout.Label("🔥 가공 및 완성");
        GUILayout.Space(10);

        // 기존의 가공/섞기/오븐 로직 그대로 유지
        if (workbench.Count == 1) DrawSingleProcessing();
        else if (workbench.Count > 1) DrawMixing();

        GUILayout.FlexibleSpace();
        if (workbench.Count > 0) DrawOven();

        GUILayout.EndVertical();
    }

    // ==========================================
    // 🏪 상점 시스템 그리기
    // ==========================================
    void DrawShopSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4f - 20));
        GUILayout.Label("🛒 상점 (구매)");
        GUILayout.Space(10);

        // 1. 초콜릿 구매 버튼 (30G)
        GUI.enabled = currentMoney >= 30; // 돈이 모자라면 회색으로 비활성화
        if (GUILayout.Button("🍫 초콜릿 (30G)", GUILayout.Height(50)))
        {
            currentMoney -= 30;
            inventory.Add(new Ingredient("초콜릿"));
            logMessage = "상점에서 초콜릿을 구매했습니다!";
        }

        GUILayout.Space(10);

        // 2. 브라우니 레시피 구매 버튼 (100G)
        GUI.enabled = currentMoney >= 100;
        if (GUILayout.Button("📜 브라우니 레시피 (100G)", GUILayout.Height(50)))
        {
            currentMoney -= 100;
            // State를 "Recipe"로 지정하여 일반 재료와 다르게 취급
            inventory.Add(new Ingredient("브라우니 레시피", "Recipe"));
            logMessage = "새로운 레시피를 구매했습니다! 창고에서 읽어보세요.";
        }

        GUI.enabled = true; // 이후 UI 그리기 원상복구
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