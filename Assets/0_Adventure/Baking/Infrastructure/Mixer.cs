using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Mixer : MonoBehaviour
{
    [SerializeField] CraftingManager craftingManager;
    [SerializeField] List<string> currentIngredients = new();

    PlayerInventory? nearbyPlayer;

    void Update()
    {
        if (nearbyPlayer == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!nearbyPlayer.HasItem) return;

            var item = nearbyPlayer.ClearItem();
            if (item != null)
            {
                currentIngredients.Add(item);
                Debug.Log($"[믹서] 재료 투입: {item} (현재 재료 수: {currentIngredients.Count}개)");
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentIngredients.Count == 0) return;
            if (nearbyPlayer.HasItem)
            {
                Debug.Log("[믹서] 플레이어의 손이 비어있지 않아 조합 결과를 받을 수 없습니다.");
                return;
            }

            var result = craftingManager.Mix(currentIngredients);
            if (result != null)
            {
                currentIngredients.Clear();
                nearbyPlayer.SetItem(result);
                Debug.Log($"[믹서] 조합 완성 및 전달: {result}");
            }
            else
            {
                Debug.Log("[믹서] 일치하는 레시피가 없어 섞지 못했습니다.");
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