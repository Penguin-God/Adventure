using System;
using System.Collections.Generic;
using UnityEngine;

public class BakingPrototype : MonoBehaviour
{
    List<string> inventory = new List<string> { "밀가루", "물", "이스트" };
    List<string> workbench = new List<string>();
    string logMessage = "재료를 선택해 작업대에 올리세요!";

    void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));
        GUILayout.Label($"[상태창] {logMessage}", GUILayout.Height(40));
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        // 1. 화면을 세 구역으로 나누어 각각의 그리기 함수 호출
        DrawInventorySection();
        DrawWorkbenchSection();
        DrawActionSection();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // ==========================================
    // 🛠️ UI 구역별 그리기 함수
    // ==========================================

    void DrawInventorySection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("📦 창고");
        GUILayout.Space(10);

        for (int i = 0; i < inventory.Count; i++)
        {
            string item = inventory[i];
            // 재사용 함수 호출: 텍스트, 버튼 이름, 클릭 시 실행할 동작(람다식)
            DrawItemRow(item, "올리기 ➔", () =>
            {
                inventory.RemoveAt(i);
                workbench.Add(item);
                logMessage = $"{item}을(를) 작업대에 올렸습니다.";
            });
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("➕ 재료 리필", GUILayout.Height(40)))
        {
            inventory.Clear();
            inventory.AddRange(new string[] { "밀가루", "물", "이스트" });
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
            string item = workbench[i];
            DrawItemRow(item, "✖ 취소", () =>
            {
                workbench.RemoveAt(i);
                inventory.Add(item);
                logMessage = $"{item}을(를) 창고로 되돌렸습니다.";
            });
        }
        GUILayout.EndVertical();
    }

    void DrawActionSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🔥 가공 및 완성");
        GUILayout.Space(10);

        // 작업대 상태에 따라 세부 가공 함수 호출
        if (workbench.Count == 1) DrawSingleProcessing();
        else if (workbench.Count > 1) DrawMixing();

        GUILayout.FlexibleSpace();
        if (workbench.Count > 0) DrawOven();

        GUILayout.EndVertical();
    }

    // ==========================================
    // ♻️ 재사용 및 세부 로직 함수
    // ==========================================

    // 중복되는 '텍스트 + 옆에 달린 버튼'을 그리는 재사용 함수
    void DrawItemRow(string itemName, string buttonText, Action onClick)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(itemName);
        if (GUILayout.Button(buttonText, GUILayout.Width(100), GUILayout.Height(50)))
        {
            onClick?.Invoke(); // 버튼이 눌리면 전달받은 로직 실행
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    void DrawSingleProcessing()
    {
        string target = workbench[0];
        if (GUILayout.Button("🔥 불 가열", GUILayout.Height(60)))
        {
            workbench[0] = target switch
            {
                "물" => "끓인 물",
                "밀가루" => "구운 밀가루",
                _ => target
            };
            logMessage = $"{target}에 불을 가했습니다.";
        }
        GUILayout.Space(10);
        if (GUILayout.Button("🦠 발효", GUILayout.Height(60)))
        {
            workbench[0] = target == "이스트" ? "발효된 이스트" : target;
            logMessage = $"{target}을(를) 발효시켰습니다.";
        }
    }

    void DrawMixing()
    {
        if (GUILayout.Button("🥣 재료 섞기", GUILayout.Height(80)))
        {
            if (workbench.Contains("밀가루") && workbench.Contains("물") && workbench.Count == 2)
                UpdateWorkbench("반죽", "밀가루와 물을 섞어 [반죽]을 만들었습니다!");
            else if (workbench.Contains("반죽") && workbench.Contains("발효된 이스트") && workbench.Count == 2)
                UpdateWorkbench("발효 반죽", "반죽에 발효된 이스트를 섞어 [발효 반죽]이 되었습니다!");
            else
                UpdateWorkbench("괴식 덩어리", "재료들이 엉망으로 섞여버렸습니다...");
        }
    }

    void DrawOven()
    {
        if (GUILayout.Button("🔥 오븐에 굽기", GUILayout.Height(80)))
        {
            if (workbench.Contains("발효 반죽") && workbench.Count == 1)
                logMessage = "[대성공] 겉바속촉 완벽한 빵이 구워졌습니다!";
            else if (workbench.Contains("반죽") && workbench.Count == 1)
                logMessage = "[성공] 딱딱하고 질긴 빵이 구워졌습니다.";
            else
                logMessage = "[실패] 오븐에서 끔찍한 숯덩이가 나왔습니다...";

            workbench.Clear();
        }
    }

    // 작업대 업데이트와 메시지 출력을 동시에 묶은 유틸리티 함수
    void UpdateWorkbench(string newItem, string message)
    {
        workbench.Clear();
        workbench.Add(newItem);
        logMessage = message;
    }
}