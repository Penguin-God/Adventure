using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EvidenceButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;

    public void Setup(string text, bool interactable, UnityAction onClick)
    {
        if (label != null) label.text = text;

        button.interactable = interactable;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }
}
