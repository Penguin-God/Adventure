using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public string? CurrentItem { get; private set; }

    public bool HasItem => !string.IsNullOrEmpty(CurrentItem);

    public void SetItem(string item)
    {
        CurrentItem = item;
    }

    public string? ClearItem()
    {
        var item = CurrentItem;
        CurrentItem = null;
        return item;
    }
}