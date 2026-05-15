using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] InteractType interactType;
    public List<string> npcDialogues;

    [Header("Dependencies")]
    public DialogueManager dialogueManager;

    public InteractType Type => interactType;

    public void Interact()
    {
        dialogueManager.StartDialogue(npcDialogues);
    }
}