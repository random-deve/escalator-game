using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggler : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Wiggle());
    }

    IEnumerator Wiggle()
    {
        float elapsed = 0.0f;
        Vector3 targetPos = transform.localPosition + new Vector3(Random.Range(-50f, 50f), Random.Range(-1f, 1f), Random.Range(-50f, 50f));
        Vector3 startPos = transform.localPosition;
        while (elapsed < 1f) 
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Wiggle());
    }
}