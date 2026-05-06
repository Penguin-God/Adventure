using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class CombineClickHandler : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;                              // 클릭 레이캐스트에 사용할 카메라
    public CombinationManager combinationManager;          // 조합 로직을 수행하는 매니저

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;            // 선택 시 하이라이트 색상

    [Header("Feedback Settings")]
    public LineRenderer lineRenderer;                      // 선택된 두 단서를 연결하는 라인 렌더러
    public float feedbackDelay = 0.5f;                     // 조합 전 피드백 대기 시간

    private ClueBehaviour firstClue;                       // 첫 번째 단서
    private ClueBehaviour secondClue;                      // 두 번째 단서

    private Renderer firstRenderer;
    private Color firstOriginalColor;

    private Renderer secondRenderer;
    private Color secondOriginalColor;

    void Reset()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    [SerializeField] bool isCombine = false;
    CombinationRuleSO currentRule = null;
    public void StartCombination(CombinationRuleSO rule)
    {
        currentRule = rule;
        isCombine = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isCombine)
        {
            TrySelectClue();
        }
    }

    private void TrySelectClue()
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            var clue = hit.collider.GetComponent<ClueBehaviour>();
            if (clue != null)
            {
                HandleClueClick(clue);
            }
        }
    }

    private void HandleClueClick(ClueBehaviour clue)
    {
        if (firstClue == null)
        {
            // 첫 번째 단서 선택 및 하이라이트
            firstClue = clue;
            ApplyHighlight(ref firstRenderer, ref firstOriginalColor, clue);
        }
        else if (firstClue == clue)
        {
            // 같은 단서 다시 클릭 시 선택 해제
            ClearFirstSelection();
        }
        else
        {
            // 두 번째 단서 선택
            secondClue = clue;
            ApplyHighlight(ref secondRenderer, ref secondOriginalColor, clue);

            // 피드백: 라인 그리기 및 조합 실행
            if (lineRenderer != null)
                DrawConnectionLine();

            StartCoroutine(CombineWithFeedback());
        }
    }

    private IEnumerator CombineWithFeedback()
    {
        // 잠시 피드백 대기
        yield return new WaitForSeconds(feedbackDelay);

        // 조합 실행
        ExecuteCombine();
    }

    private void ExecuteCombine()
    {
        // combinationManager.Combine(firstClue, secondClue);
        combinationManager.Combine(currentRule, firstClue, secondClue);
        // 하이라이트 및 라인 제거
        ClearLine();
        ClearFirstSelection();
        ClearSecondSelection();

        // 상태 초기화
        firstClue = null;
        secondClue = null;
        currentRule = null;
        isCombine = false;
    }

    private void DrawConnectionLine()
    {
        if (firstClue != null && secondClue != null)
        {
            lineRenderer.SetPosition(0, firstClue.transform.position);
            lineRenderer.SetPosition(1, secondClue.transform.position);
            lineRenderer.enabled = true;
        }
    }

    private void ClearLine()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void ApplyHighlight(ref Renderer rend, ref Color originalColor, ClueBehaviour clue)
    {
        rend = clue.GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
            rend.material.color = highlightColor;
        }
    }

    private void ClearFirstSelection()
    {
        if (firstRenderer != null)
        {
            firstRenderer.material.color = firstOriginalColor;
            firstRenderer = null;
        }
        firstClue = null;
    }

    private void ClearSecondSelection()
    {
        if (secondRenderer != null)
        {
            secondRenderer.material.color = secondOriginalColor;
            secondRenderer = null;
        }
        secondClue = null;
    }
}
