using System.Collections.Generic;
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
        if (dialogueManager == null || dayManager == null || dialogueData == null)
        {
            Debug.LogWarning("필요한 매니저 또는 SO 데이터가 연결되지 않았습니다.");
            return;
        }

        int dayIndex = dayManager.CurrentDay - 1;
        List<string> todayLines = dialogueData.GetDialogueText(dayIndex);

        if (todayLines != null && todayLines.Count > 0) dialogueManager.StartDialogue(todayLines);
    }
}