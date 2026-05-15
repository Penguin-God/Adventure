using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BedInteract : MonoBehaviour, IInteractable
{
    [Header("Dependencies")]
    public DayManager dayManager;

    public InteractType Type => InteractType.Input;
    public void Interact() => dayManager.GoToNextDay();
}