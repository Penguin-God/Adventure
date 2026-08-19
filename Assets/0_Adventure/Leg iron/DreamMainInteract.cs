using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DreamMainInteract : MonoBehaviour
{
    [Header("Dialogues")]
    public List<string> normalTexts; // 1. 리모컨 조작 전 기본 대사
    public List<string> changedTexts; // 2. 리모컨 조작 후 진짜 대사

    [Header("Scale Settings")]
    public float scaleMultiplier = 1.5f; // 커질 배수
    public float scaleDuration = 0.5f;

    [Header("Dependencies")]
    public DialogueManager dialogueManager;
    public DreamManager dreamManager; // 꿈에서 깨우기 위한 매니저

    private bool isUnlocked = false;  // 리모컨에 의해 해금되었는가?
    private bool isCompleted = false; // 모든 이벤트가 끝났는가?

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        if (isCompleted) return;
        StartCoroutine(Co_Dialogue());
    }

    IEnumerator Co_Dialogue()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isUnlocked)
        {
            // 아직 리모컨을 안 만졌다면 기본 대사만 출력
            // 기본 대사는 특별한 연출이 없으므로 이벤트를 구독하지 않습니다.
            dialogueManager.StartDialogue(normalTexts);
        }
        else
        {
            // 혹시 모를 중복 구독을 방지하기 위해 먼저 해제합니다.
            dialogueManager.onLineAdvanced -= OnLineAdvanced;
            dialogueManager.OnDialogueFinished -= WakeUpFromDream;

            // 진실된 대화가 시작될 때 이벤트를 구독(+=) 합니다.
            dialogueManager.onLineAdvanced += OnLineAdvanced;
            dialogueManager.OnDialogueFinished += WakeUpFromDream;

            // 바뀐 대사 출력 시작
            dialogueManager.StartDialogue(changedTexts);
        }
    }

    // --- 리모컨이 호출하는 함수 ---
    public void UnlockTrueForm()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(SmoothScale(transform.localScale * scaleMultiplier));
    }

    // 매니저의 onLineAdvanced 이벤트가 발생할 때마다 실행됨
    void OnLineAdvanced()
    {
        float step = (scaleMultiplier - 1f);
        Vector3 newScale = transform.localScale + (originalScale * step);

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(SmoothScale(newScale));
    }

    // 매니저의 OnDialogueFinished 이벤트가 발생할 때 실행됨
    void WakeUpFromDream()
    {
        // 대화가 끝났으므로, 이후 다른 오브젝트의 대화에서 내가 커지지 않도록 반드시 구독을 해제(-=) 합니다.
        dialogueManager.onLineAdvanced -= OnLineAdvanced;
        dialogueManager.OnDialogueFinished -= WakeUpFromDream;

        isCompleted = true;
        dreamManager.WakeUp();
    }

    IEnumerator SmoothScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float time = 0;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / scaleDuration);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}