using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemSubmitter : MonoBehaviour
{
    [SerializeField] string targetItem;
    
    PlayerInventory nearbyPlayer;

    void Update()
    {
        if (nearbyPlayer == null || !Input.GetKeyDown(KeyCode.E)) return;

        if (!nearbyPlayer.HasItem)
        {
            Debug.Log($"[{gameObject.name}] 제출할 아이템을 들고 있지 않습니다. (필요한 아이템: {targetItem})");
            return;
        }

        if (nearbyPlayer.CurrentItem == targetItem)
        {
            Debug.Log($"[{gameObject.name}] 정답! 올바른 아이템 제출 완료: {targetItem}");
            nearbyPlayer.ClearItem();
        }
        else
        {
            Debug.Log($"[{gameObject.name}] 오답! 제출한 아이템: {nearbyPlayer.CurrentItem} / 기대한 아이템: {targetItem}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerInventory>(out var player)) nearbyPlayer = player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerInventory>(out var player) && nearbyPlayer == player) nearbyPlayer = null;
    }
}