using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Transformer : MonoBehaviour
{
    [SerializeField] CraftingManager craftingManager;
    [SerializeField] ProcessType processType;

    PlayerInventory? nearbyPlayer;

    void Update()
    {
        if (nearbyPlayer == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!nearbyPlayer.HasItem) return;

            var currentItem = nearbyPlayer.CurrentItem!;
            var result = craftingManager.Transform(currentItem, processType);

            if (result != null)
            {
                nearbyPlayer.SetItem(result);
                Debug.Log($"[{processType} 기기] 변환 완료: {result}");
            }
            else
            {
                Debug.Log($"[{processType} 기기] 변환 가능한 레시피가 아닙니다: {currentItem}");
            }
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