using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(FixedDialogueInteract))]
public class DialogueInteractOneTime : MonoBehaviour, IInteractable
{
    bool isShow;
    public InteractType Type => InteractType.CollisionOnce;

    public void Interact()
    {
        if (isShow) return;

        GetComponent<FixedDialogueInteract>().Interact();
        isShow = true;
    }
}
