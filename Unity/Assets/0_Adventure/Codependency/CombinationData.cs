using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combination", menuName = "CoDependnecy/Combination Data")]
public class CombinationData : ScriptableObject
{
    public List<string> requiredChoiceIDs; // 예: "A", "F" 순서대로 기록되었을 때
    public List<string> specialDialogue; // 출력할 특수 대사
}