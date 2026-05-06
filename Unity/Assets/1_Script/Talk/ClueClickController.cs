using UnityEngine;

public class ClueClickController : MonoBehaviour
{
    Camera cam;

    [Tooltip("Expansion 매니저 참조")]
    public ClueExpansionManager expansionManager;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                var clue = hit.collider.GetComponent<ClueBehaviour>();
                if (clue != null)
                {
                    expansionManager.Reveal(clue);
                }
            }
        }
    }
}
