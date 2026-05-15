using UnityEngine;

public enum InteractType
{
    Collision,
    Input,
}

public interface IInteractable
{
    InteractType Type { get; }
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
    [SerializeField] bool isInteract;

    IInteractable currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
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