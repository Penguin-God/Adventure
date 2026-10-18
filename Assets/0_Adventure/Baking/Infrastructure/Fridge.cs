using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fridge : MonoBehaviour
{
    [SerializeField] string[] availableIngredients = { "물", "밀가루", "우유" };

    [SerializeField] PlayerInventory nearbyPlayer;

    void Update()
    {
        if (nearbyPlayer == null) return;

        if (nearbyPlayer.HasItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var returnedItem = nearbyPlayer.ClearItem();
                Debug.Log($"[냉장고] 재료 반납 완료: {returnedItem} (현재 들고 있는 재료 없음)");
            }
            return;
        }

        for (int i = 0; i < availableIngredients.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                var selectedItem = availableIngredients[i];
                nearbyPlayer.SetItem(selectedItem);
                Debug.Log($"[냉장고] {i + 1}번 재료 꺼냄: {selectedItem}");
                break;
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