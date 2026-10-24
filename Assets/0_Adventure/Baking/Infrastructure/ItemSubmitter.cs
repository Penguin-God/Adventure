using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemSubmitter : MonoBehaviour
{
    [SerializeField] LevelTracker npcLevelTracker;
    [SerializeField] bool consumeOnSuccess = true;

    PlayerInventory nearbyPlayer;

    void Awake() => npcLevelTracker = GetComponent<LevelTracker>();

    void Update()
    {
        if (nearbyPlayer == null || !Input.GetKeyDown(KeyCode.E)) return;

        if (!nearbyPlayer.HasItem)
        {
            Debug.Log($"[{gameObject.name}] 줄 수 있는 음식이 없습니다.");
            return;
        }

        var item = nearbyPlayer.CurrentItem;
        if (consumeOnSuccess) nearbyPlayer.ClearItem();

        npcLevelTracker.AddExpByItem(item);
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