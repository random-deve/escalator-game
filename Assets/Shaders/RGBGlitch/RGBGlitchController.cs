using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RGBGlitchController : MonoBehaviour
{
    public RGBGlitchFeature glitchFeature;
    public float minGlitchDuration = 0.1f;
    public float maxGlitchDuration = 0.5f;
    public float minGlitchInterval = 1.0f;
    public float maxGlitchInterval = 3.0f;
    public float glitchAmount = 0.5f;

    private float nextGlitchTime = 0f;
    private bool isGlitching = false;

    void Update()
    {
        if (Time.time >= nextGlitchTime)
        {
            isGlitching = !isGlitching;
            if (isGlitching)
            {
                nextGlitchTime = Time.time + Random.Range(minGlitchDuration, maxGlitchDuration);
                glitchFeature.settings.glitchAmount = glitchAmount;
            }
            else
            {
                nextGlitchTime = Time.time + Random.Range(minGlitchInterval, maxGlitchInterval);
                glitchFeature.settings.glitchAmount = 0.0f;
            }
        }

        if (isGlitching)
        {
            glitchFeature.Create();
        }
    }
}
