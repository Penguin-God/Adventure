using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BakingGameUI : MonoBehaviour
{
    // --- [상태 (State)] ---
    // 핵심 로직은 '순수 함수'지만, 게임 화면은 진행 상황을 기억해야 하므로 상태를 가집니다.
    private List<Ingredient> inventory = new List<Ingredient>();
    private List<Ingredient> workbench = new List<Ingredient>();
    private List<Recipe> recipes = new List<Recipe>();

    private string logMessage = "재료를 선택해 작업대에 올리세요!";

    void Start()
    {
        // 1. 초기 레시피 세팅
        recipes.Add(new Recipe("완벽한 빵 (Perfect Bread)", new HashSet<Ingredient>
        {
            new Ingredient("Flour", "Raw"),
            new Ingredient("Water", "Boiled"),
            new Ingredient("Yeast", "Fermented")
        }));

        // 2. 초기 인벤토리(창고) 기본 재료 제공
        inventory.Add(new Ingredient("Flour", "Raw"));
        inventory.Add(new Ingredient("Water", "Raw"));
        inventory.Add(new Ingredient("Yeast", "Raw"));
    }

    // 유니티 내장 GUI 시스템 (코드로만 화면에 UI를 렌더링)
    void OnGUI()
    {
        // UI 폰트 크기 세팅
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        // 전체 UI 영역 설정 (화면 꽉 차게, 여백 20)
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));

        // [상단 로그 메세지]
        GUILayout.Label($"[상태창] {logMessage}", GUILayout.Height(40));
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        // ==========================================
        // 1. 창고 (Inventory) 영역
        // ==========================================
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("📦 창고 (Inventory)");
        GUILayout.Space(10);

        foreach (var item in inventory.ToList()) // 순회 중 리스트 수정을 위해 ToList() 사용
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{item.Name}\n({item.State})");

            // 버튼 클릭 시 상태 업데이트
            if (GUILayout.Button("올리기 ➔", GUILayout.Width(100), GUILayout.Height(50)))
            {
                inventory.Remove(item);
                workbench.Add(item);
                logMessage = $"{item.Name}을(를) 작업대에 올렸습니다.";
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("➕ 기본 재료 리필", GUILayout.Height(40)))
        {
            inventory.Add(new Ingredient("Flour", "Raw"));
            inventory.Add(new Ingredient("Water", "Raw"));
            inventory.Add(new Ingredient("Yeast", "Raw"));
        }
        GUILayout.EndVertical();

        // ==========================================
        // 2. 작업대 (Workbench) 영역
        // ==========================================
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🛠️ 작업대 (Workbench)");
        GUILayout.Space(10);

        for (int i = 0; i < workbench.Count; i++)
        {
            var item = workbench[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{item.Name}\n({item.State})");

            if (GUILayout.Button("✖ 취소", GUILayout.Width(80), GUILayout.Height(50)))
            {
                workbench.RemoveAt(i);
                inventory.Add(item); // 다시 창고로 복귀
                logMessage = $"{item.Name}을(를) 창고로 되돌렸습니다.";
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();

        // ==========================================
        // 3. 가공 및 오븐 (Actions) 영역
        // ==========================================
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3f - 30));
        GUILayout.Label("🔥 가공 및 완성");
        GUILayout.Space(10);

        // 단일 재료 가공 로직 (작업대에 아이템이 딱 1개일 때만 활성화)
        if (workbench.Count == 1)
        {
            var target = workbench[0];
            if (GUILayout.Button("불 가열 (Apply Heat)", GUILayout.Height(60)))
            {
                // ✨ 함수형 코어 호출: 새 상태의 객체를 반환받아 기존 자리를 덮어씁니다. (원본 훼손 없음)
                workbench[0] = Processor.ApplyHeat(target);
                logMessage = $"{target.Name}에 불을 가했습니다.";
            }
            GUILayout.Space(10);
            if (GUILayout.Button("발효 (Ferment)", GUILayout.Height(60)))
            {
                workbench[0] = Processor.Ferment(target);
                logMessage = $"{target.Name}을(를) 발효시켰습니다.";
            }
        }
        else if (workbench.Count > 1)
        {
            GUILayout.Label("단일 가공은 재료가\n1개일 때만 가능합니다.");
        }
        else
        {
            GUILayout.Label("작업대에 재료를 올려주세요.");
        }

        GUILayout.FlexibleSpace();

        // 오븐 로직 (작업대에 재료가 1개 이상일 때)
        if (workbench.Count > 0)
        {
            if (GUILayout.Button("오븐에 굽기 (Bake!)", GUILayout.Height(80)))
            {
                // ✨ 함수형 코어 호출: 여러 재료를 받아 '새로운 반죽' 생성
                var dough = Dough.Mix(workbench.ToArray());

                // ✨ 함수형 코어 호출: 반죽을 오븐에 넣어 문자열 결과 반환
                string result = Oven.Bake(dough, recipes);

                logMessage = $"[결과] {result} 완성!!";

                // 완성했으니 작업대를 비움
                workbench.Clear();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}