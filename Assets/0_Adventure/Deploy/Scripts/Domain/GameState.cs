using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingSaveData
{
    public string id;
    public string buildingName; // To look up the SO
    public int x;
    public int y;
    
    // Additional state if needed, though mostly starting at 0 is fine according to doc.
}

public static class GameState
{
    public static int unlockedStage = 1;
    public static int currentPlayingStage = 1;
    public static int currentGold = 3000;
    
    public static List<BuildingSaveData> savedBuildings = new List<BuildingSaveData>();
    
    public static void SaveGrid(IEnumerable<BuildingModel> buildings)
    {
        savedBuildings.Clear();
        foreach (var b in buildings)
        {
            savedBuildings.Add(new BuildingSaveData
            {
                id = b.id,
                buildingName = b.data.buildingName,
                x = b.x,
                y = b.y
            });
        }
    }
}
