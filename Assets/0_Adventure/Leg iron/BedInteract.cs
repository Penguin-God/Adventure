using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BedInteract : MonoBehaviour, IInteractable
{
    public Transform playerTransform;
    public Transform dreamLocation;
    public DayManager dayManager;
    public TransitionManager transitionManager;

    public InteractType Type => InteractType.Input;

    public void Interact()
    {
        dayManager.GoToNextDay();
        transitionManager.StartTransition(playerTransform, dreamLocation);

        StartCoroutine(DreamSetupSequence());
    }

    IEnumerator DreamSetupSequence()
    {
        yield return new WaitForSeconds(transitionManager.fadeDuration);
        playerTransform.GetComponent<TopDownPlayer>().SetDreamMode(true);
    }
}