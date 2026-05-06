using UnityEngine;

[CreateAssetMenu(menuName = "Clue/Combination Rule")]
public class CombinationRuleSO : ScriptableObject
{
    public ClueTypeSO clueA;
    public ClueTypeSO clueB;
    public MapEventSO[] events;  // 여러 이벤트를 순서대로 배열 관리
}
