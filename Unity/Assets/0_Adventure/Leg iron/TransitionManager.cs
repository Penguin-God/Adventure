using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [Header("UI Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    public void StartTransition(Transform player, Transform destination) => StartCoroutine(TransitionSequence(player, destination));

    IEnumerator TransitionSequence(Transform player, Transform destination)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        if (player != null && destination != null)
        {
            player.position = destination.position;
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 마지막에 목표 투명도로 확실히 고정합니다.
        color.a = endAlpha;
        fadeImage.color = color;
    }
}