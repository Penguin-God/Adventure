using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))] // 줄 시각화를 위한 컴포넌트 추가
public class SpiderSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float jumpForce = 15f;         // 스페이스바 도약 힘
    public float searchRadius = 10f;      // 앵커를 탐색할 반경
    public LayerMask anchorLayer;         // 앵커들이 속해있는 레이어

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer lineRenderer;
    private bool isSwinging = false;
    private Transform currentAnchor;      // 현재 연결된 앵커 추적

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();

        joint.enabled = false;
        lineRenderer.enabled = false; // 시작할 때는 줄 숨김
    }

    void Update()
    {
        // 1. 줄 날리기 (마우스 왼쪽 클릭)
        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            FindAndAttachWeb();
        }

        // 2. 줄 끊고 위로 도약하기 (Space 키)
        if (Input.GetKeyDown(KeyCode.Space) && isSwinging)
        {
            DetachAndJump();
        }

        // 3. 매 프레임마다 줄의 시작점과 끝점 업데이트
        if (isSwinging && currentAnchor != null)
        {
            lineRenderer.SetPosition(0, transform.position);     // 줄의 시작 (플레이어)
            lineRenderer.SetPosition(1, currentAnchor.position); // 줄의 끝 (앵커)
        }
    }

    private void FindAndAttachWeb()
    {
        // 내 주변(searchRadius)에 있는 anchorLayer 속성의 모든 콜라이더를 찾음
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius, anchorLayer);

        Transform closestAnchor = null;
        float closestDistance = Mathf.Infinity;
        print(colliders.Length);
        // 찾은 앵커들 중 가장 가까운 앵커 판별
        foreach (Collider2D coll in colliders)
        {
            float distance = Vector2.Distance(transform.position, coll.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAnchor = coll.transform;
            }
        }

        // 반경 내에 앵커가 하나도 없다면 줄 날리기 취소
        if (closestAnchor == null) return;

        // 가장 가까운 앵커로 타겟 설정 및 스윙 시작
        currentAnchor = closestAnchor;
        isSwinging = true;
        joint.enabled = true;
        lineRenderer.enabled = true; // 줄 보이기 시작

        joint.connectedAnchor = currentAnchor.position;
        joint.distance = Vector2.Distance(transform.position, currentAnchor.position);
    }

    private void DetachAndJump()
    {
        isSwinging = false;
        joint.enabled = false;
        lineRenderer.enabled = false; // 줄 끊기면 선 숨김
        currentAnchor = null;

        Vector2 jumpDirection = (rb.linearVelocity.normalized + Vector2.up * 1.5f).normalized;
        rb.linearVelocity = rb.linearVelocity * 0.5f;
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }

    // 에디터에서 앵커 탐색 반경을 시각적으로 확인할 수 있게 해주는 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}