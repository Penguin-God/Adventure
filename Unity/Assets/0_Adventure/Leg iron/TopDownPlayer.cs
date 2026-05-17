using UnityEngine;

public enum InteractType
{
    Collision,
    Input,
    CollisionOnce,
}

public interface IInteractable
{
    InteractType Type { get; }
    void Interact();
}


[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Vector2 movement;

    [SerializeField] bool isInteract;
    IInteractable currentTarget;

    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (canMove == false)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (currentTarget != null && isInteract == false)
        {
            if (currentTarget.Type == InteractType.Collision) ExecuteInteraction();
            else if (currentTarget.Type == InteractType.Input && Input.GetKeyDown(KeyCode.Space)) ExecuteInteraction();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetDreamMode(bool isDreaming)
    {
        canMove = !isDreaming; // 꿈꾸는 중이면 조작 불가
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = !isDreaming; // 꿈꾸는 중이면 투명하게
    }
    void ExecuteInteraction()
    {
        currentTarget.Interact();
        isInteract = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IInteractable interactable = collision.collider.GetComponent<IInteractable>();
        if (interactable != null)
            currentTarget = interactable;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IInteractable interactable = collision.collider.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentTarget)
        {
            isInteract = false;
            currentTarget = null;
        }
    }
}