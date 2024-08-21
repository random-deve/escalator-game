using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretcher : MonoBehaviour
{
    public float minMod = 0.98f;
    public float maxMod = 1.02f;
    public float duration = 1f;

    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
        StartCoroutine(Stretch(duration));
    }

    private IEnumerator Stretch(float duration)
    {
        float elapsed = 0.0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = initialScale * Random.Range(minMod, maxMod);
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
            elapsed += Time.deltaTime;
        }
        StartCoroutine(Stretch(duration));
    }
}
