// ClueExpansionRuleSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Clue/Expansion Rule")]
public class ClueExpansionRuleSO : ScriptableObject
{
    [Tooltip("이 룰이 적용될 부모 Clue의 ID")]
    public string parentClueID;

    [System.Serializable]
    public class ChildEntry
    {
        [Tooltip("씬에 미리 배치된 자식 Clue의 ID")]
        public string childClueID;
    }

    [Tooltip("부모 공개 시 드러낼 자식 목록")]
    public ChildEntry[] children;
}
