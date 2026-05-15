using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class TeleportInteract : MonoBehaviour, IInteractable
{
    [Header("Teleport Settings")]
    public Transform targetPosition;
    Transform playerTransform;

    public InteractType Type => InteractType.Collision;

    void Start()
    {
        playerTransform = FindAnyObjectByType<TopDownPlayer>().transform;
    }

    public void Interact() => playerTransform.position = targetPosition.position;
}