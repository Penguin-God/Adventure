using UnityEngine;

// ==========================================
// [상태 및 UI 관리자] 부수효과 처리 구역
// ==========================================
public class ClickerGameUI : MonoBehaviour
{
    [Header("확률 설정 (Inspector)")]
    [Range(0f, 100f)] public float forestTier2DropChance = 5.0f;
    [Range(0f, 100f)] public float mineTier2DropChance = 5.0f;

    // 초기 상태 세팅: 숲 데이터, 광산 데이터, 궁극 아이템 순서
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

        GUILayout.Label("🛠️ 지형 조합 클리커 - 다중 지형 시스템");
        GUILayout.Space(20);

        // 1. 인벤토리 구역 (숲과 광산을 좌우로 배치)
        GUILayout.BeginHorizontal();
        DrawTerrainInventory("🌲 숲 인벤토리", currentState.Forest);
        GUILayout.Space(20);
        DrawTerrainInventory("⛰️ 광산 인벤토리", currentState.Mine);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // 궁극 아이템 보유 현황
        GUILayout.BeginVertical("box");
        DrawResourceRow("🌟 궁극의 아티팩트", currentState.UltimateItem);
        GUILayout.EndVertical();

        GUILayout.Space(40);

        // 2. 행동 구역 (숲과 광산 클릭/조합 버튼을 좌우로 배치)
        GUILayout.BeginHorizontal();
        DrawTerrainActions(
            "🌲 숲에서 행동", currentState.Forest, forestTier2DropChance,
            (random, chance) => currentState = ClickerLogic.GatherForest(currentState, random, chance),
            () => currentState = ClickerLogic.CraftForestTier2(currentState),
            () => currentState = ClickerLogic.CraftForestTier3(currentState)
        );
        GUILayout.Space(20);
        DrawTerrainActions(
            "⛰️ 광산에서 행동", currentState.Mine, mineTier2DropChance,
            (random, chance) => currentState = ClickerLogic.GatherMine(currentState, random, chance),
            () => currentState = ClickerLogic.CraftMineTier2(currentState),
            () => currentState = ClickerLogic.CraftMineTier3(currentState)
        );
        GUILayout.EndHorizontal();

        GUILayout.Space(40);

        // 3. 최종 조합 버튼
        DrawCraftButton(
            "✨ 궁극의 아티팩트 만들기 (숲 Tier 3 [1개] + 광산 Tier 3 [1개] 소모)",
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
        DrawResourceRow("기본 재료 (Tier 1)", state.Tier1);
        DrawResourceRow("가공 재료 (Tier 2)", state.Tier2);
        DrawResourceRow("고급 재료 (Tier 3)", state.Tier3);
        GUILayout.EndVertical();
    }

    // 람다식을 매개변수(Action)로 받아 UI 중복을 완벽히 제거한 함수
    private void DrawTerrainActions(
        string title, TerrainState state, float dropChance,
        System.Action<float, float> onGather,
        System.Action onCraftT2,
        System.Action onCraftT3)
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label(title);
        GUILayout.Space(10);

        if (GUILayout.Button($"⛏️ 채집 (기본+1 / T2 {dropChance}%)", GUILayout.Height(60)))
        {
            float rawRandomValue = UnityEngine.Random.Range(0f, 100f);
            onGather?.Invoke(rawRandomValue, dropChance);
        }
        GUILayout.Space(10);

        DrawCraftButton("🔨 T2 가공 (T1 100개)", state.Tier1 >= 100, onCraftT2);
        DrawCraftButton("✨ T3 고급 (T2 100개)", state.Tier2 >= 100, onCraftT3);

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
        if (GUILayout.Button(label, GUILayout.Height(60)))
        {
            onCraft?.Invoke();
        }
        GUI.enabled = true;
        GUILayout.Space(10);
    }
}