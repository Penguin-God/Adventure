using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapEvent/Replace Object")]
public class ReplaceEventSO : MapEventSO
{
    public string activeId;
    public string removeId;

    public override void Execute(ClueManager clueManager) => clueManager.ChangeClue(activeId, removeId);
}
