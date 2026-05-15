using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueText;

    private List<string> currentLines = new List<string>();
    private int currentLineIndex = 0;
    public event Action OnDialogueFinished;
    public event Action onLineAdvanced;

    void Update()
    {
        if (dialoguePanel.activeSelf && DialogueInput())
        {
            ShowNextLine();
        }
    }

    bool DialogueInput() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);

    public void StartDialogue(List<string> lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            dialogueText.text = currentLines[currentLineIndex];
            currentLineIndex++;
            onLineAdvanced?.Invoke();
        }
        else
        {
            dialoguePanel.SetActive(false);
            OnDialogueFinished?.Invoke();
        }
    }
}