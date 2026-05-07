using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText; // TMPro 사용

    private List<string> currentLines = new List<string>();
    private int currentLineIndex = 0;

    // 대사가 완전히 끝났을 때 외부(GameManager)로 알려주는 이벤트
    public Action OnDialogueFinished;

    void Update()
    {
        // 패널이 켜져 있을 때 마우스 좌클릭 감지
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