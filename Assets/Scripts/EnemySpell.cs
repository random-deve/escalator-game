using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpell : MonoBehaviour
{
    [Header("General Settings")]
    public bool die;
    public float dieAfter;
    private float timer;
    public string damageTag;

    [Header("Damage Settings")]
    public float hitDamage = 10f;
    public float stayDamage = 10f;
    public float startDamageAfter = 0f;
    public float stopDamageAfter = 0f;
    public bool continuousDamage = true;
    public bool collisionDamage;

    [Header("Effect Settings")]
    public bool dying = false;
    public float deathEffectDelay;
    public GameObject deathEffect;
    public CameraShake shaker;
    public float shakeAmount = 2f;

    private void Awake()
    {
        timer = dieAfter;
        dying = false;
        shaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
        shaker.Shake(0.3f, shakeAmount, shakeAmount);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= deathEffectDelay && die && !dying)
        {
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
            dying = true;
        }
        if (timer <= 0 && die)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(damageTag) && timer <= dieAfter - startDamageAfter && (stopDamageAfter == 0 || timer >= dieAfter - stopDamageAfter))
        {
            if (continuousDamage && other.GetComponent<PlayerController>())
            {
                other.GetComponent<PlayerController>().TakeDamage(stayDamage * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(damageTag) && timer <= dieAfter - startDamageAfter && (stopDamageAfter == 0 || timer >= dieAfter - stopDamageAfter))
        {
            if (other.GetComponent<PlayerController>())
            {
                other.GetComponent<PlayerController>().TakeDamage(hitDamage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(damageTag) && collisionDamage)
        {
            if (collision.gameObject.GetComponent<PlayerController>())
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(hitDamage);
            }
        }
    }
}
