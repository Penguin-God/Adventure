using System.Collections.Generic;
using UnityEngine;

public class InputManger : MonoBehaviour
{
    List<Clue> selected = new List<Clue>();
    SimlualationManager simlualationManager;
    Camera cam;

    void Start()
    {
        simlualationManager = FindAnyObjectByType<SimlualationManager>();
        cam = Camera.main;
    }

    public void OnClueClicked(Clue clue)
    {
        if (clue == null || selected.Contains(clue)) return;
        selected.Add(clue);

        if (selected.Count == 2)
            Check();
    }

    void Check() => simlualationManager.TryProgressSimulation(selected[0].id, selected[1].id);

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
                OnClueClicked(hit.collider.GetComponent<Clue>());
        }
    }
}
