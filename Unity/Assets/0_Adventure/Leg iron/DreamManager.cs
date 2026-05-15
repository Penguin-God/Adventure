using System.Collections;
using UnityEngine;

public class DreamManager : MonoBehaviour
{
    [Header("Wake Up Settings")]
    public Transform playerTransform;
    public Transform wakeUpLocation;

    [Header("Dependencies")]
    public TransitionManager transitionManager;

    public void WakeUp()
    {
        transitionManager.StartTransition(playerTransform, wakeUpLocation);
        StartCoroutine(WakeUpSequence());
    }

    IEnumerator WakeUpSequence()
    {
        yield return new WaitForSeconds(transitionManager.fadeDuration);

        TopDownPlayer player = playerTransform.GetComponent<TopDownPlayer>();
        if (player != null) player.SetDreamMode(false);
    }
}