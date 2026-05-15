using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class TeleportInteract : MonoBehaviour, IInteractable
{
    [Header("Teleport Settings")]
    public Transform targetPosition; // 이동할 목표 위치
    public Transform playerTransform;

    void Start()
    {
        playerTransform = FindAnyObjectByType<TopDownPlayer>().transform;
    }

    public void Interact() => playerTransform.position = targetPosition.position;
}