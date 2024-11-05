using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    public float health;

    public float damage;
    public float stayDamage;
    public bool dieOnDeath = true;

    public bool dead;
    public GameObject deathEffect;
    public GameObject hitEffect;

    public AudioSource source;
    public AudioClip deathSfx;

    public float hitEffectCooldown = 0.2f;
    public float lastHitEffectTime;

    public HealthBar healthBar;

    public bool accepted;
    private AcceptanceBoss acceptedStats;

    private void Awake()
    {
        lastHitEffectTime = -hitEffectCooldown;
        if (healthBar)
        {
            healthBar.maxHealth = maxHealth;
            healthBar.SetHealth(health);
        }
        source = GameObject.FindObjectOfType<AudioSource>();
        acceptedStats = GetComponent<AcceptanceBoss>();
        if (acceptedStats != null)
            accepted = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (healthBar != null)
            healthBar.SetHealth(health);
        if (hitEffect && Time.time >= lastHitEffectTime + hitEffectCooldown)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            lastHitEffectTime = Time.time;
        }
        if (health <= 0 && !accepted)
            Die();
        else if (health <= 0 && accepted && acceptedStats.phase < 3)
        {
            health = 10000f;
            acceptedStats.phase += 1;
            if (!acceptedStats.phase2AttackDone)
                acceptedStats.state = "phase 2";
            else if (!acceptedStats.phase3AttackDone)
                acceptedStats.state = "phase 3";
        } else if (health <= 0 && accepted && acceptedStats.phase == 3)
            Die();
    }

    public void Die()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().TakeDamage(-maxHealth / 10f);
        source.PlayOneShot(deathSfx);

        dead = true;
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (dieOnDeath)
            Destroy(gameObject);
        else if (!gameObject.GetComponent<AcceptancePillar>())
            gameObject.SetActive(false);
    }
}
