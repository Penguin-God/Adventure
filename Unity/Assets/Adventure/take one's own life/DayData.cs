using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ButtonActionData
{
    public int buttonIndex;
    [TextArea(2, 5)]
    public string[] dialogueLines;
}

[CreateAssetMenu(fileName = "New Day Data", menuName = "Day Data")]
public class DayData : ScriptableObject
{
    public string dateName;
    public List<ButtonActionData> activeButtons;
}