using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool shaking;

    public IEnumerator TheShake(float duration, float magnitudeX, float magnitudeY)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitudeX;
            float y = Random.Range(-1f, 1f) * magnitudeY;

            Vector3 targetPos = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        shaking = false;
    }

    public void Shake(float duration, float magnitudeX, float magnitudeY)
    {
        if (!shaking)
        {
            StartCoroutine(TheShake(duration, magnitudeX, magnitudeY));
            shaking = true;
        }
    }
}
