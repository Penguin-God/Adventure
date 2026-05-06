using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Managers")]
    public DialogueManager dialogueManager;

    [Header("UI References")]
    public TextMeshProUGUI dateText;
    public Button[] normalButtons;
    public Button nextDayButton;

    [Header("Game Data")]
    public List<DayData> dayDatas; // 이제 인스펙터에서 ScriptableObject 에셋들을 드래그해서 넣게 됩니다.

    private int currentDayIndex = 0;
    private HashSet<int> clickedButtonsToday = new HashSet<int>();

    void Start()
    {
        SetupDay(currentDayIndex);
    }

    void SetupDay(int dayIndex)
    {
        if (dayIndex >= dayDatas.Count)
        {
            Debug.Log("모든 날짜를 클리어했습니다!");
            return;
        }

        clickedButtonsToday.Clear();
        DayData todayData = dayDatas[dayIndex];
        dateText.text = todayData.dateName;

        for (int i = 0; i < normalButtons.Length; i++)
        {
            int captureIndex = i;
            normalButtons[i].onClick.RemoveAllListeners();

            ButtonActionData actionData = todayData.activeButtons.Find(x => x.buttonIndex == captureIndex);

            if (actionData != null)
            {
                normalButtons[i].interactable = true;
                normalButtons[i].image.color = Color.white;
                normalButtons[i].onClick.AddListener(() => OnNormalButtonClicked(captureIndex, actionData.dialogueLines));
            }
            else
            {
                normalButtons[i].interactable = false;
                normalButtons[i].image.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }

        nextDayButton.interactable = false;
        nextDayButton.image.color = new Color(0.5f, 0.5f, 0.5f);
        nextDayButton.onClick.RemoveAllListeners();
        nextDayButton.onClick.AddListener(OnNextDayButtonClicked);
    }

    void OnNormalButtonClicked(int btnIndex, string[] lines)
    {
        // 1. 다이얼로그 매니저에게 대사 출력 요청
        dialogueManager.StartDialogue(lines);

        // 2. 버튼 클릭 기록 및 다음 날 버튼 활성화 체크
        if (!clickedButtonsToday.Contains(btnIndex))
        {
            clickedButtonsToday.Add(btnIndex);

            // 클릭한 버튼 즉시 비활성화 및 색상 변경
            normalButtons[btnIndex].interactable = false;
            normalButtons[btnIndex].image.color = new Color(0.5f, 0.5f, 0.5f);

            CheckAllButtonsClicked();
        }
    }

    void CheckAllButtonsClicked()
    {
        DayData todayData = dayDatas[currentDayIndex];
        if (clickedButtonsToday.Count >= todayData.activeButtons.Count)
        {
            nextDayButton.interactable = true;
            nextDayButton.image.color = Color.white;
        }
    }

    public string[] endingDialogue;
    void OnNextDayButtonClicked()
    {
        // 🔥 수정된 부분: 현재 날짜가 세팅된 날짜 데이터의 마지막인지 체크합니다.
        if (currentDayIndex >= dayDatas.Count - 1)
        {
            // 마지막 날이라면 다음 날로 넘어가지 않고 다이얼로그 매니저를 통해 대사를 출력합니다.
            dialogueManager.StartDialogue(endingDialogue);
        }
        else
        {
            // 마지막 날이 아니라면 정상적으로 다음 날로 넘어갑니다.
            currentDayIndex++;
            SetupDay(currentDayIndex);
        }
    }
}