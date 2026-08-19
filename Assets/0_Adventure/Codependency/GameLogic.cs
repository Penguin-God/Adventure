using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    public int CurrentDayIndex { get; private set; }
    private List<string> choiceHistory = new List<string>();

    private List<DayData> allDays;
    private List<CombinationData> allCombinations;

    // 생성자를 통해 데이터를 주입받음
    public GameLogic(List<DayData> days, List<CombinationData> combinations)
    {
        allDays = days;
        allCombinations = combinations;
        CurrentDayIndex = 0;
    }

    // 선택 기록 저장
    public void RecordChoice(string choiceID)
    {
        choiceHistory.Add(choiceID);
    }

    // 현재 상태와 방금 한 선택을 바탕으로 올바른 대사 반환
    public List<string> GetDialogueForCurrentState(ChoiceData currentChoice)
    {
        // 이번 선택을 포함한 가상의 기록 생성
        List<string> tempHistory = new List<string>(choiceHistory);
        tempHistory.Add(currentChoice.choiceID);

        // 조합 데이터 검사
        foreach (var combo in allCombinations)
        {
            // 누적된 선택이 특수 조합의 요구 조건과 정확히 일치하는지 확인
            if (combo.requiredChoiceIDs.SequenceEqual(tempHistory))
            {
                return combo.specialDialogue; // 특수 대사 반환
            }
        }

        return currentChoice.defaultDialogue; // 조합이 없다면 기본 대사 반환
    }

    // 다음 날로 넘어가기
    public void AdvanceDay()
    {
        CurrentDayIndex++;
    }

    // 현재 날짜의 데이터 가져오기
    public DayData GetCurrentDayData()
    {
        if (CurrentDayIndex >= allDays.Count) return null; // 게임 종료
        return allDays[CurrentDayIndex];
    }
}