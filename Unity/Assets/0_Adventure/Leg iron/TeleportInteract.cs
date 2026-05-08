using UnityEngine;

public class TeleportInteract : MonoBehaviour, IInteractable
{
    [Header("Teleport Settings")]
    public Transform targetPosition; // 이동할 목표 위치
    public Transform playerTransform; // 인스펙터에서 직접 연결할 플레이어

    public void Interact() => playerTransform.position = targetPosition.position;
}