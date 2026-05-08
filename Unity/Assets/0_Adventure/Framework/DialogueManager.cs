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
    public Action OnDialogueFinished;

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(List<string> lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            dialogueText.text = currentLines[currentLineIndex];
            currentLineIndex++;
        }
        else
        {
            // 대사 종료
            dialoguePanel.SetActive(false);
            OnDialogueFinished?.Invoke(); // 구독하고 있는 로직 실행
        }
    }
}