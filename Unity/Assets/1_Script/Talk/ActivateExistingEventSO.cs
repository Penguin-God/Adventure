using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapEvent/Activate Existing")]
public class ActivateExistingEventSO : MapEventSO
{
    public string targetID;

    public override void Execute(ClueManager clueManager) => clueManager.ActiveClue(targetID);
}
