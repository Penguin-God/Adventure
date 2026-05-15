using UnityEngine;

public interface IInteractable
{
    void Interact();
}


[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Interaction Settings")]
    public float interactionCooldown = 0.5f; // 무한 반복을 막기 위한 쿨다운 (0.5초)
    private float lastInteractTime; // 마지막으로 상호작용을 한 시간

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. 이동 입력 받기
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // 2. 부딪혔을 때 자동으로 실행되는 유니티 내장 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 현재 시간과 마지막 상호작용 시간을 비교하여, 쿨다운이 지나지 않았다면 함수를 종료(return)합니다.
        if (Time.time - lastInteractTime < interactionCooldown) return;

        IInteractable interactable = collision.collider.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // 상호작용 실행 후, 마지막 상호작용 시간을 현재 시간으로 갱신합니다.
            interactable.Interact();
            lastInteractTime = Time.time;
        }
    }
}