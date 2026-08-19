using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DailyDialogue
{
    public List<string> lines;
}

[CreateAssetMenu(fileName = "New Dialogue Database", menuName = "My/Dialogue Database")]
public class DialogueDatabaseSO : ScriptableObject
{
    [Header("날짜별 대사 데이터")]
    public List<DailyDialogue> dailyDialogues;
    public List<string> GetDialogueText(int index) => dailyDialogues[index].lines;
}