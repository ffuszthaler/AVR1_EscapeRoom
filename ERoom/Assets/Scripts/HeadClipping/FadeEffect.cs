using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    private float fadeDelay = 0.07f;
    private Material material;

    private bool isFadingOut = false;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    public void Fade(bool fadeOut)
    {
        if (fadeOut && isFadingOut)
            return;
        if (!fadeOut && !isFadingOut)
            return;
        isFadingOut = fadeOut;
        StopAllCoroutines();
        string val = isFadingOut ? "OUT" : "in";
        Debug.Log($"Starting fade {val} coroutine");
        StartCoroutine(PlayEffect(fadeOut));
    }

    private IEnumerator PlayEffect(bool fadeOut)
    {
        float startAlpha = material.GetFloat("_Alpha");
        float endAlpha = fadeOut ? 1.0f : 0.0f;
        float remainingTime
            = fadeDelay * Mathf.Abs(endAlpha - startAlpha);

        float elapsedTime = 0;
        while (elapsedTime < fadeDelay)
        {
            elapsedTime += Time.deltaTime;
            float tempVal = Mathf.Lerp(startAlpha, endAlpha,
                elapsedTime / remainingTime);

            material.SetFloat("_Alpha", tempVal);
            yield return null;
        }
        material.SetFloat("_Alpha", endAlpha);
    }
}
