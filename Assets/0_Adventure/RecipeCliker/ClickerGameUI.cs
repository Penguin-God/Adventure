using UnityEngine;

// ==========================================
// [상태 및 UI 관리자] 부수효과 처리 구역
// ==========================================
public class ClickerGameUI : MonoBehaviour
{
    [Header("🌲 숲 확률 설정 (Inspector)")]
    [Range(0f, 100f)] public float forestTier2Chance = 15.0f; // 15%
    [Range(0f, 100f)] public float forestTier3Chance = 5.0f;  // 5%

    [Header("⛰️ 광산 확률 설정 (Inspector)")]
    [Range(0f, 100f)] public float mineTier2Chance = 20.0f;
    [Range(0f, 100f)] public float mineTier3Chance = 2.0f;

    private ClickerState currentState = new ClickerState(
        new TerrainState(0, 0, 0),
        new TerrainState(0, 0, 0),
        0
    );

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));
        GUILayout.Label("🛠️ 지형 조합 클리커 - 단일 드롭 확률 시스템");
        GUILayout.Space(20);

        // 1. 인벤토리 구역
        GUILayout.BeginHorizontal();
        DrawTerrainInventory("🌲 숲 인벤토리", currentState.Forest);
        GUILayout.Space(20);
        DrawTerrainInventory("⛰️ 광산 인벤토리", currentState.Mine);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginVertical("box");
        DrawResourceRow("🌟 궁극의 아티팩트", currentState.UltimateItem);
        GUILayout.EndVertical();
        GUILayout.Space(40);

        // 2. 행동 구역 (확률 데이터 전달)
        GUILayout.BeginHorizontal();
        DrawTerrainActions(
            "🌲 숲에서 행동", currentState.Forest, forestTier2Chance, forestTier3Chance,
            (random, t2, t3) => currentState = ClickerLogic.GatherForest(currentState, random, t2, t3),
            () => currentState = ClickerLogic.CraftForestTier2(currentState),
            () => currentState = ClickerLogic.CraftForestTier3(currentState)
        );
        GUILayout.Space(20);
        DrawTerrainActions(
            "⛰️ 광산에서 행동", currentState.Mine, mineTier2Chance, mineTier3Chance,
            (random, t2, t3) => currentState = ClickerLogic.GatherMine(currentState, random, t2, t3),
            () => currentState = ClickerLogic.CraftMineTier2(currentState),
            () => currentState = ClickerLogic.CraftMineTier3(currentState)
        );
        GUILayout.EndHorizontal();
        GUILayout.Space(40);

        // 3. 최종 조합 버튼
        DrawCraftButton(
            "✨ 궁극의 아티팩트 만들기 (숲 T3 [1개] + 광산 T3 [1개] 소모)",
            currentState.Forest.Tier3 >= 1 && currentState.Mine.Tier3 >= 1,
            () => currentState = ClickerLogic.CraftUltimate(currentState)
        );
        GUILayout.EndArea();
    }

    // --- [중복 UI 로직 재사용 함수] ---
    private void DrawTerrainInventory(string title, TerrainState state)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);
        DrawResourceRow("기본 (T1)", state.Tier1);
        DrawResourceRow("가공 (T2)", state.Tier2);
        DrawResourceRow("고급 (T3)", state.Tier3);
        GUILayout.EndVertical();
    }

    // 확률 파라미터가 추가된 행동 렌더링 함수
    private void DrawTerrainActions(
        string title, TerrainState state, float t2Chance, float t3Chance,
        System.Action<float, float, float> onGather,
        System.Action onCraftT2,
        System.Action onCraftT3)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);

        // 버튼 텍스트에 확률을 직관적으로 표기
        float t1Chance = 100f - (t2Chance + t3Chance);
        if (GUILayout.Button($"⛏️ 채집\n(T3 {t3Chance}% / T2 {t2Chance}% / T1 {t1Chance}%)", GUILayout.Height(80)))
        {
            float rawRandomValue = UnityEngine.Random.Range(0f, 100f);
            onGather?.Invoke(rawRandomValue, t2Chance, t3Chance);
        }
        GUILayout.Space(10);

        DrawCraftButton("🔨 T2 조합 (T1 100개)", state.Tier1 >= 100, onCraftT2);
        DrawCraftButton("✨ T3 조합 (T2 100개)", state.Tier2 >= 100, onCraftT3);

        GUILayout.EndVertical();
    }

    private void DrawResourceRow(string name, int amount)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{amount} 개");
        GUILayout.EndHorizontal();
    }

    private void DrawCraftButton(string label, bool canCraft, System.Action onCraft)
    {
        GUI.enabled = canCraft;
        if (GUILayout.Button(label, GUILayout.Height(60))) onCraft?.Invoke();
        GUI.enabled = true;
        GUILayout.Space(10);
    }
}