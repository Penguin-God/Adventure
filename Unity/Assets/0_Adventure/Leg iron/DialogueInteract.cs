using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueInteract : MonoBehaviour, IInteractable
{
    public List<string> npcDialogues;

    [Header("Dependencies")]
    public DialogueManager dialogueManager;

    public void Interact()
    {
        dialogueManager.StartDialogue(npcDialogues);
    }
}