using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ChoiceData // 개별 선택지 데이터
{
    public string choiceID; // 예: "A", "B" (조합 체크용)
    public string buttonText; // 버튼에 표시될 텍스트
    public List<string> defaultDialogue; // 기본 대사
}

// 하루치 데이터 (해당 날짜에 나올 선택지들)
[CreateAssetMenu(fileName = "New Day", menuName = "CoDependnecy/Day Data")]
public class DayData : ScriptableObject
{
    public List<ChoiceData> choices;
}