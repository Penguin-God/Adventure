using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FixedDialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] InteractType interactType;
    [SerializeField] List<string> dialogues;

    [Header("Dependencies")]
    public DialogueManager dialogueManager;

    public InteractType Type => interactType;
    public void Interact() => dialogueManager.StartDialogue(dialogues);
}