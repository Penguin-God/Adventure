using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SO_DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] InteractType interactType;

    public DialogueDatabaseSO dialogueData;

    public DialogueManager dialogueManager;
    public DayManager dayManager;

    public InteractType Type => interactType;

    public void Interact()
    {
        int dayIndex = dayManager.CurrentDay - 1;
        dialogueManager.StartDialogue(dialogueData.GetDialogueText(dayIndex));
    }
}