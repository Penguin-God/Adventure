using UnityEngine;

public class ClueHover : MonoBehaviour
{
    Camera cam;
    ClueBehaviour lastClue;  // 마지막에 호버 중이던 단서 추적

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        // 레이어 최적화 하고 싶으면 LayerMask 써서 Clue 레이어만 걸러내세요
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            var clue = hit.collider.GetComponent<ClueBehaviour>();
            if (clue != null)
            {
                if (clue != lastClue)
                {
                    // 새로운 클루로 호버 변경
                    TooltipManager.Instance.Show(clue.type.clueName);
                    lastClue = clue;
                }
                return;
            }
        }

        // Clue 위에 없으면 툴팁 숨김
        if (lastClue != null)
        {
            TooltipManager.Instance.Hide();
            lastClue = null;
        }
    }
}
