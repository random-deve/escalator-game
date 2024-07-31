using UnityEngine;

public class PulsatingLight : MonoBehaviour
{
    public Light _light;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float pulseSpeed = 2f;

    void Update()
    {
        float intensity = minIntensity + Mathf.PingPong(Time.time * pulseSpeed, maxIntensity - minIntensity);
        _light.intensity = intensity;
    }
}
