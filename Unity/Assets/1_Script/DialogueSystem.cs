using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Linq;

[Serializable]
public struct DialogueData
{
    public string id;
    [TextArea(3, 3)]
    public string[] texts;
}

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;
    public bool isDialoging;
    [SerializeField] string currentDialogueId;

    [SerializeField] DialogueData[] dialogueDatas;
    Dictionary<string, bool> dialogueSeenFlag;

    void Awake()
    {
        dialogueSeenFlag = dialogueDatas.ToDictionary(x => x.id, x => false);
        sentences = new Queue<string>();
        dialogueUI.SetActive(false);
    }

    public bool IsSeen(string id) => dialogueSeenFlag[id];

    public void StartDialogue(string dialogueId)
    {
        currentDialogueId = dialogueId;
        StartDialogue(dialogueDatas.First(x => x.id == dialogueId).texts);
    }

    void StartDialogue(string[] dialogue)
    {
        if (isDialoging) return;

        isDialoging = true;
        dialogueUI.SetActive(true);
        sentences.Clear();
        foreach (string sentence in dialogue)
            sentences.Enqueue(sentence);
        DisplayNextSentence();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        dialogueSeenFlag[currentDialogueId] = true;
        currentDialogueId = "";
        isDialoging = false;
        dialogueUI.SetActive(false);
    }
}
