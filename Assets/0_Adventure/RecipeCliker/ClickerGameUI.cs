using UnityEngine;

public class ClickerGameUI : MonoBehaviour
{
    // 게임 내 유일한 가변 상태입니다.
    private ClickerState currentState = new ClickerState(0, 0, 0);

    [Range(0f, 100f)]
    public float tier2DropChance = 5.0f;

    private void OnGUI()
    {
        // 폰트 크기 세팅
        GUI.skin.label.fontSize = 24;
        GUI.skin.button.fontSize = 24;

        // 전체 UI 영역 설정 (화면 가득 채우기, 여백 50)
        GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));

        GUILayout.Label("🛠️ 지형 조합 클리커 프로토타입");
        GUILayout.Space(20);

        // 분리된 UI 그리기 함수들 호출
        DrawResourceSection();
        GUILayout.Space(40);
        DrawActionSection();

        GUILayout.EndArea();
    }

    // --- [UI 렌더링 구역 분리] ---

    private void DrawResourceSection()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("📦 인벤토리 현황");
        GUILayout.Space(10);

        // 재사용 함수를 통한 리소스 UI 렌더링
        DrawResourceRow("기본 재료 (Tier 1)", currentState.Tier1);
        DrawResourceRow("가공 재료 (Tier 2)", currentState.Tier2);
        DrawResourceRow("고급 재료 (Tier 3)", currentState.Tier3);

        GUILayout.EndVertical();
    }

    private void DrawActionSection()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("⚡ 행동");
        GUILayout.Space(10);

        // 1. 기본 클릭 버튼
        if (GUILayout.Button($"⛏️ 지형 클릭 (기본 +1 / Tier 2 {tier2DropChance}% 확률)", GUILayout.Height(60)))
        {
            // 부수효과: 유니티 난수 생성기에서 0~100 사이의 '숫자'만 뽑아냅니다.
            float rawRandomValue = UnityEngine.Random.Range(0f, 100f);

            // 순수 함수 호출: 로직에게 판단에 필요한 모든 데이터(현재 상태, 뽑은 난수, 기준 확률)를 넘깁니다.
            currentState = ClickerLogic.Gather(currentState, rawRandomValue, tier2DropChance);
        }
        GUILayout.Space(20);

        // 2. 조합 버튼
        DrawCraftButton(
            "🔨 가공 재료 만들기 (기본 100개 소모)",
            currentState.Tier1 >= 100,
            () => currentState = ClickerLogic.CraftTier2(currentState)
        );

        DrawCraftButton(
            "✨ 고급 재료 만들기 (가공 100개 소모)",
            currentState.Tier2 >= 100,
            () => currentState = ClickerLogic.CraftTier3(currentState)
        );

        GUILayout.EndVertical();
    }
    
    // 텍스트와 숫자를 좌우로 배치하는 UI 함수
    private void DrawResourceRow(string name, int amount)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        GUILayout.FlexibleSpace(); // 중간 여백을 자동으로 밀어줌
        GUILayout.Label($"{amount} 개");
        GUILayout.EndHorizontal();
    }

    // 조건에 따라 활성화/비활성화되는 조합 버튼 UI 함수
    private void DrawCraftButton(string label, bool canCraft, System.Action onCraft)
    {
        GUI.enabled = canCraft; // true면 클릭 가능, false면 회색으로 비활성화

        if (GUILayout.Button(label, GUILayout.Height(60)))
        {
            onCraft?.Invoke(); // 전달받은 순수 함수 로직 실행
        }

        GUI.enabled = true; // 이후 UI가 모두 비활성화되는 것을 막기 위해 원상복구
        GUILayout.Space(10);
    }
}