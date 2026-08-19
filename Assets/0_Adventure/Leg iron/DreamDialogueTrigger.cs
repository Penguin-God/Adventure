using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DreamDialogueTrigger : MonoBehaviour
{
    public List<string> texts;
    public DialogueManager dialogueManager;

    void OnMouseDown() => StartCoroutine(Co_Dialogue());

    IEnumerator Co_Dialogue() // 다이어로그 매니저 내부 Input이랑 중복 실행되서 딜레이
    {
        yield return new WaitForSeconds(0.1f);
        dialogueManager.StartDialogue(texts);
    }
}