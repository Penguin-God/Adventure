using UnityEngine;

public class CombineTrigger : MonoBehaviour
{
    [SerializeField] CombinationRuleSO combinationRule;
    void OnMouseUp()
    {
        FindAnyObjectByType<CombineClickHandler>().StartCombination(combinationRule);
    }
}
