using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Data Setup")]
    public List<DayData> dayDatas;
    public List<CombinationData> combinationDatas;

    [Header("UI References")]
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public DialogueManager dialogueManager;

    private GameLogic logic;

    void Start()
    {
        logic = new GameLogic(dayDatas, combinationDatas);
        // 대사가 끝났을 때 실행될 행동(다음 날로 진행)을 구독
        dialogueManager.OnDialogueFinished += HandleDialogueFinished;
        ShowCurrentDayChoices();
    }

    void ShowCurrentDayChoices()
    {
        ClearButtons();
        DayData currentDay = logic.GetCurrentDayData();

        if (currentDay == null)
        {
            Debug.Log("모든 날짜가 끝났습니다. 게임 클리어!");
            return;
        }

        // 선택지 개수에 맞게 버튼 동적 생성
        foreach (var choice in currentDay.choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.buttonText;

            // 버튼 클릭 이벤트 연결
            btnObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(ChoiceData choice)
    {
        ClearButtons();

        // 로직 클래스에 문의하여 출력할 대사를 가져오고 선택 기록 저장
        List<string> dialogueToPlay = logic.GetDialogueForCurrentState(choice);
        logic.RecordChoice(choice.choiceID);
        dialogueManager.StartDialogue(dialogueToPlay);
    }

    void HandleDialogueFinished()
    {
        // 대사가 끝났으므로 자동으로 날짜를 넘기고 새로운 버튼 세팅
        logic.AdvanceDay();
        ShowCurrentDayChoices();
    }

    void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);
    }
}