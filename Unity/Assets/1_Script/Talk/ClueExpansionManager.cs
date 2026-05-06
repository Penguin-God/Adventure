using UnityEngine;

public class ClueExpansionManager : MonoBehaviour
{
    [Tooltip("씬에 배치된 시작 단서 (ID로도 일치시켜야 함)")]
    public ClueBehaviour startClue;

    [Tooltip("모든 Expansion Rule SO")]
    public ClueExpansionRuleSO[] rules;

    [Tooltip("기본 숨김 알파 (0 = 완전 안 보임)")]
    [Range(0, 1)] public float defaultHiddenAlpha = 0f;

    [SerializeField] GameObject line;

    // ID → ClueBehaviour 맵
    [SerializeField] ClueManager clueManager;
    void Start()
    {
        foreach (var cb in clueManager.Clues)
            cb.gameObject.SetActive(false);

        // 2) 시작 단서 공개 & 자식 노출
        if (startClue != null)
            Reveal(startClue);
    }



    public void Reveal(ClueBehaviour clue)
    {
        float startAhapa = 0.3f;
        clue.Reveal(1f);

        // 룰 찾아서 자식들만 알파 세팅 + 선 그리기
        foreach (var rule in rules)
        {
            if (rule.parentClueID != clue.ID) continue;
            foreach (var child in rule.children)
            {
                if (clueManager.ClueDict.TryGetValue(child.childClueID, out var cb))
                {
                    cb.Reveal(startAhapa);
                    DrawLine(clue.transform.position, cb.transform.position);
                }
                else
                    Debug.Log($"ClueExpansionRule에서 찾을 수 없는 childID: {child.childClueID}");
            }
            break;
        }
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        var lr = Instantiate(line).GetComponent<LineRenderer>();
        // 위치 지정
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
