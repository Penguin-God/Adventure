using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    [SerializeField] ClueManager manager;
    public void Combine(CombinationRuleSO rule, ClueBehaviour c1, ClueBehaviour c2)
    {
        bool matches = (rule.clueA == c1.type && rule.clueB == c2.type) || (rule.clueA == c2.type && rule.clueB == c1.type);
        if (matches == false) return;

        foreach (var ev in rule.events)
            ev.Execute(manager);
    }
}
