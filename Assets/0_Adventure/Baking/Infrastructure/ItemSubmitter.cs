using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemSubmitter : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    [SerializeField] string targetItem = "빵";
    [SerializeField] bool consumeOnSuccess = true;

    PlayerInventory? nearbyPlayer;

    void Update()
    {
        if (nearbyPlayer == null || !Input.GetKeyDown(KeyCode.E)) return;

        if (!nearbyPlayer.HasItem)
        {
            Debug.Log($"[{gameObject.name}] 제출할 아이템이 없습니다. (필요: {targetItem})");
            return;
        }

        if (nearbyPlayer.CurrentItem == targetItem)
        {
            Debug.Log($"[{gameObject.name}] 제출 성공: {targetItem}");
            levelManager.AddExpByItem(targetItem);
            if (consumeOnSuccess) nearbyPlayer.ClearItem();
        }
        else
        {
            Debug.Log($"[{gameObject.name}] 오답! 제출한 아이템: {nearbyPlayer.CurrentItem}");
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