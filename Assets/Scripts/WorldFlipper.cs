using System.Collections;
using UnityEngine;

public class WorldFlipper : MonoBehaviour
{
    public GameObject level;
    public PlayerController player;

    public bool random;
    [Range(0, 1)] public float chance;

    public bool interval;
    public float minInterval;
    public float maxInterval;

    public bool flipping;

    public float nextFlip = 10;
    private float timer;

    public float flipDuration = 1.0f;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            player.canGrapple = false;
        }
        else
        {
            player.canGrapple = true;
        }

        if (Time.time >= nextFlip && interval)
        {
            nextFlip = Time.time + Random.Range(minInterval, maxInterval);
            StartCoroutine(SmoothFlip());
        }
        else if (random && Random.Range(0, 1f) <= chance)
        {
            StartCoroutine(SmoothFlip());
        }
    }

    private IEnumerator SmoothFlip()
    {
        if (flipping) yield break;

        flipping = true;

        Quaternion startRotation = level.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(level.transform.eulerAngles.x, level.transform.eulerAngles.y, level.transform.eulerAngles.z + 180);

        float elapsedTime = 0;

        while (elapsedTime < flipDuration)
        {
            level.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        level.transform.rotation = endRotation;
        flipping = false;

        player.EndGrapple();
        player.jumpForce *= -1;
    }
}
