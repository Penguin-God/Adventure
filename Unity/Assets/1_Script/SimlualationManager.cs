using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct SimulationData
{
    public int sumulationId;
    [SerializeField] int _id1;
    [SerializeField] int _id2;
    public bool Matches(int id1, int id2) => Ids.Contains(id1) && Ids.Contains(id2);
    public IReadOnlyList<int> Ids => new List<int>() { _id1, _id2 };

    public GameObject[] removeClues;
    public GameObject[] spawnClues;

    public void SuccessSimulation()
    {
        foreach (var item in removeClues)
            GameObject.Destroy(item);
        foreach (var item in spawnClues)
            item.SetActive(true);
    }
}


public class SimlualationManager : MonoBehaviour
{
    [SerializeField] SimulationData[] simulationDatas;
    public event Action<SimulationData> OnSimulationSuccess;

    public void TryProgressSimulation(int id1, int id2)
    {
        foreach (var data in simulationDatas)
        {
            if(data.Matches(id1, id2))
                data.SuccessSimulation();
        }
    }
}