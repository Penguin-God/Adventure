using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    // 외부(GameManager)에서 대사를 전달하며 호출하는 함수
    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        currentLineIndex = 0;

        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeSelf) ShowNextLine();
    }

    void ShowNextLine()
    {
        // 남은 대사가 있다면 출력
        if (currentLines != null && currentLineIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLineIndex];
            currentLineIndex++;
        }
        else
        {
            // 대사가 끝나면 패널 닫기
            dialoguePanel.SetActive(false);
        }
    }
}