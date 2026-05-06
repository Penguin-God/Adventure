using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    [SerializeField] List<ClueBehaviour> clues;
    public IReadOnlyCollection<ClueBehaviour> Clues => clues;
    public Dictionary<string, ClueBehaviour> ClueDict => clues.ToDictionary(x => x.type.id, x => x);

    void Awake()
    {
        clues = GetComponentsInChildren<ClueBehaviour>().ToList();
    }

    public void ActiveClue(string id) => ClueDict[id].gameObject.SetActive(true);


    public void RemoveClue(string id)
    {
        Destroy(ClueDict[id].gameObject);
        clues.Remove(ClueDict[id]);
    }

    public void ChangeClue(string activeId, string removeId)
    {
        ActiveClue(activeId);
        RemoveClue(removeId);
    }
}
