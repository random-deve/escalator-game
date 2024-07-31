using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float health;
    public float maxHealth;
    public Gradient gradient;
    public Image fill;

    public void Awake()
    {
        slider.maxValue = 1f;
        slider.minValue = 0f;
    }

    public void SetHealth(float health)
    {
        slider.value = health / maxHealth;
        if (fill)
            fill.color = gradient.Evaluate(slider.value);
    }
}
