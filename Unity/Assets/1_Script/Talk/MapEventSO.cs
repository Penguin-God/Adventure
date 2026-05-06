using System.Collections.Generic;
using UnityEngine;

public abstract class MapEventSO : ScriptableObject
{
    public abstract void Execute(ClueManager clueManager);
}
