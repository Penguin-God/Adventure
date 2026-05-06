using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapEvent/Delete Event")]
public class DeleteEventSO : MapEventSO
{
    public string targetId;

    public override void Execute(ClueManager clueManager) => clueManager.RemoveClue(targetId);
}
