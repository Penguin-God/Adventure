using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DreamRemoteInteract : MonoBehaviour
{
    public List<string> texts;

    [Header("Dependencies")]
    public DialogueManager dialogueManager;
    public DreamMainInteract mainObject; // 영향을 줄 메인 오브젝트를 직접 참조

    private bool isCompleted = false;

    void OnMouseDown()
    {
        if (isCompleted) return;
        StartCoroutine(Co_Dialogue());
    }

    IEnumerator Co_Dialogue()
    {
        yield return new WaitForSeconds(0.1f);
        dialogueManager.StartDialogue(texts);
        mainObject.UnlockTrueForm();
    }
}