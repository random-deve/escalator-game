using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The plan:
    Inputs:
        Player position
        Player grounded?
        Player grappling?
        
        Own health
        Own position

    Behavior:
        10% chance -> dash to player and combo
        Player too far -> approach or ranged attack (1% to approach bc wtf)
        Player too close -> create distance or melee attack
        Player grappling  -> Go to player target location or create distance (50%)
        Player in range and in air -> dash and melee
        
        Health < 50% && player in range or too close -> spam melee attacks and dashes
        Health < 50% && player too far -> summon swarm of enemies and mash ranged attacks
        Health < 25% -> fly up and start ultimate (exposion but big) or summon a few minibosses and come back down once theyre defeated

        When attacking, dont move
        Combo means dash and melee multiple times
        Dash means reset velocity and set velocity to a high value to get to player - this must have a target position and not the player's active position so the attacks are actually dodge-able;
        Melee means check in a sphere for player controller
        Ranged means repeatedly launch a few projectiles and move the target when shooting (think flying head from ultrakill)
 */


public class AcceptanceBoss : MonoBehaviour
{
    public Enemy stats;
    public PlayerController player;
    public Rigidbody rb;
    public Transform sword;
    public GameObject beacon;
    public List<GameObject> pillars;

    [Header("Info")]
    public string state = "chill";
    public int phase;
    public bool phase2AttackDone; // transition attack into phase 2 - probably something along the lines of doing something to the players movement like random teleports or an up teleport then dash
    public bool phase3AttackDone; // the big explosion probably
    public float moveForce; // addforce movement when not dashing to make sure the implosion and black hole spells work
    public float jumpForce; // also addforce movement
    public float dashSpeed; // setting velocity
    public float health;
    public Vector3 position;

    [Header("Player")]
    public bool playerGrounded;
    public bool playerGrappling;
    public Vector3 playerPosition;
    public Vector3 initialPlayerPosition;
    public Vector3 directionToPlayer;
    public float distanceToPlayer; // only use for checks on melee or ranged attacks
    public Transform attackTarget; // move this around and use this as the directional target
    // this will be like a random empty gameobject that will just move around
    // for ranged attacks it should update every time a projectile is launched, not just before the attack
    public Vector3 directionToTarget;
    public float distanceToTarget;
    
    public float meleeDistance = 3f;
    public float rangedDistance = 10f;

    [Header("Attack Stuff")]
    public float stamina;
    public float maxStamina;
    public float staminaRechargeRate;
    public LayerMask playerLayer;
    public float attackCooldown;
    [Header("Dash")]
    public float dashPauseTime = 1f;
    public float dashCloseEnoughDistance = 1f;
    public int comboAtkNum = 0;
    public float dashStaminaCost = 0.1f;
    [Header("Melee attack")]
    public float meleeTiltAngle = 30f;
    public float meleeTiltDuration = 0.05f;
    public float meleeSlashDownDuration = 0.1f;
    public float meleeRange = 2f;
    public float meleeDamage = 5f;
    public float meleeAttackingCooldown = 0.5f;
    public float meleeStaminaCost = 1f;
    [Header("Ranged Attack")]
    public Transform firepoint;
    public GameObject projectile;
    public float rangedDamage = 5f;
    public float rangedCooldown = 0.1f;
    public float rangedSpeed = 100f;
    public float rangedAttackingCooldown = 0.5f;
    public float rangedStaminaCost = 1f;

    private void Awake()
    {
        stats = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody>();
        stamina = 0f;
        initialPlayerPosition = player.transform.position;
    }

    private void Update()
    {
        playerGrounded = player.grounded;
        playerGrappling = player.isGrappling;
        health = stats.health;
        position = transform.position;
        playerPosition = player.transform.position;
        distanceToTarget = Vector3.Distance(position, attackTarget.position);
        distanceToPlayer = Vector3.Distance(position, playerPosition);
        directionToTarget = (attackTarget.position - position).normalized;
        directionToPlayer = (playerPosition - position).normalized;

        ChooseAttack();

        if (stamina < maxStamina)
            stamina += Time.deltaTime * staminaRechargeRate;

        if (phase == 1 && attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
        else if (phase <= 2)
            attackCooldown = 0f;
    }

    private void FixedUpdate()
    {
        if (state == "chill" && rb.velocity.magnitude != 0f)
        {
            // slowly stop (i am so doing this wrong sob)
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
        }
    }

    public void UpdateTargetPosition()
    {
        attackTarget.position = playerPosition;
    }

    public void ChooseAttack()
    {
        // phase 1
        if (state == "chill" && attackCooldown <= 0f && stamina >= 0.5f && phase == 1)
        {
            // placeholder testing logic
            if (Random.Range(0, 1f) < 0.1f)
                StartCoroutine(Chilling(30f));
            if (distanceToPlayer > meleeDistance && distanceToPlayer < rangedDistance)
            {
                if (stamina > 3f)
                {
                    StartCoroutine(Dash(true)); // melee combo
                    stamina -= 3f;
                }
            }
            else if (distanceToPlayer > rangedDistance)
            {
                if (Random.Range(0f, 1f) < 0.99f)
                    StartCoroutine(RangedAttack());
                else
                    StartCoroutine(Dash(false));
            }
            else if (distanceToPlayer < meleeDistance)
            {
                if (distanceToPlayer < meleeRange)
                    StartCoroutine(Melee(false));
                else
                    StartCoroutine(Dash(false));
            }
        }
        // phase 2
        else if (state == "chill" && phase == 2 && attackCooldown <= 0f)
        {
            if (Random.Range(0f, 1f) < 0.05f)
            {
                StartCoroutine(Chilling(20f));
            } else if (Random.Range(0f, 1f) < 0.5f)
            {
                StartCoroutine(Dash(true)); // melee combo
            } else
            {
                StartCoroutine(RangedAttack());
            }
        }
        // phase 3
        else if (state == "chill" && phase == 3 && attackCooldown <= 0f)
        {
            if (Random.Range(0f, 1f) < 0.05f)
            {
                StartCoroutine(Chilling(10f));
            }
            else if (Random.Range(0f, 1f) < 0.5f)
            {
                StartCoroutine(Dash(true)); // melee combo
            }
            else if (Random.Range(0f, 1f) < 0.5f)
            {
                StartCoroutine(RangedAttack());
            } else
            {
                StartCoroutine(Dash(true));
                StartCoroutine(RangedAttack());
            }
        }
        // phase changing attacks and buffs
        if (state == "phase 2" || state == "phase 3")
        {
            if (!phase2AttackDone)
            {
                // phase 2 attack
                Debug.Log("Phase 2 attack");
                transform.position = new Vector3(0, 5f, 0);
                Instantiate(beacon, position - new Vector3(0, 10f), Quaternion.Euler(-90f, 0f, 0f));
                maxStamina = 100f;
                staminaRechargeRate = 100f;
                stamina = 100f;
                rangedAttackingCooldown /= 1.5f;
                rangedSpeed *= 1.5f;
                meleeAttackingCooldown /=  1.5f;
                attackCooldown = 1.5f;
                stats.maxHealth = 1500f;
                stats.health = stats.maxHealth;
                stats.healthBar.maxHealth = stats.maxHealth;
                stats.healthBar.SetHealth(stats.health);
                foreach(GameObject pillar in pillars)
                {
                    if (pillar.GetComponent<Enemy>().dead)
                    {
                        pillar.SetActive(true);
                        pillar.GetComponent<Enemy>().dead = false;
                        pillar.GetComponent<Enemy>().TakeDamage(-pillar.GetComponent<Enemy>().maxHealth - 1f);
                    }
                }
                state = "chill";
            }
            else if (!phase3AttackDone)
            {
                // phase 3 attack
                Debug.Log("Phase 3 attack");
                player.transform.position = initialPlayerPosition;
                rangedAttackingCooldown /= 1.5f;
                meleeAttackingCooldown /= 1.1f;
                rangedDamage *= 1.3f;
                meleeDamage *= 1.3f;
                attackCooldown = 1.5f;
                stats.maxHealth = 500f;
                stats.healthBar.maxHealth = stats.maxHealth;
                stats.health = stats.maxHealth;
                stats.healthBar.SetHealth(stats.health);
                foreach (GameObject pillar in pillars)
                {
                    if (pillar.GetComponent<Enemy>().dead)
                    {
                        pillar.SetActive(true);
                        pillar.GetComponent<Enemy>().dead = false;
                        pillar.GetComponent<Enemy>().TakeDamage(-pillar.GetComponent<Enemy>().maxHealth - 1f);
                    }
                }
                state = "chill";
            }
        }
    }

    public IEnumerator Chilling(float duration)
    {
        state = "chilling fr";
        yield return new WaitForSeconds(duration);
        state = "chill";
    }

    public IEnumerator Dash(bool meleeCombo)
    {
        if (stamina < dashStaminaCost)
        {
            state = "chill";
            yield break;
        }
        stamina -= dashStaminaCost;
        state = "dashing";
        UpdateTargetPosition();
        Vector3 dashDirection = directionToTarget;
        rb.velocity = Vector3.zero;
        transform.LookAt(new Vector3(attackTarget.position.x, position.y, attackTarget.position.z));
        yield return new WaitForSeconds(dashPauseTime);
        rb.velocity = dashDirection * dashSpeed;
        while (Vector3.Distance(transform.position, attackTarget.position) > 1f)
        {
            dashDirection = directionToTarget;
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;

        if (!meleeCombo)
            state = "chill";
        else
            StartCoroutine(Melee(true));
    }

    public IEnumerator Melee(bool meleeCombo)
    {
        if (meleeCombo)
            comboAtkNum += 1;
        if (stamina < meleeStaminaCost && !meleeCombo)
        {
            state = "chill";
            yield break;
        }
        if (!meleeCombo)
            stamina -= meleeStaminaCost;
        state = "melee";

        // anticipation tilt
        Quaternion initialRotation = sword.rotation;
        Quaternion tiltUpRotation = Quaternion.Euler(sword.eulerAngles.x - meleeTiltAngle / 4f, sword.eulerAngles.y, sword.eulerAngles.z);
        for (float t = 0; t < meleeTiltDuration; t += Time.deltaTime)
        {
            sword.rotation = Quaternion.Lerp(initialRotation, tiltUpRotation, t / meleeTiltDuration);
            yield return null;
        }

        UpdateTargetPosition();
        
        // down slash
        Quaternion slashDownRotation = Quaternion.Euler(sword.eulerAngles.x + meleeTiltAngle, sword.eulerAngles.y, sword.eulerAngles.z);
        for (float t = 0; t < meleeSlashDownDuration; t += Time.deltaTime)
        {
            sword.rotation = Quaternion.Lerp(tiltUpRotation, slashDownRotation, t / meleeSlashDownDuration);
            yield return null;
        }

        // check for player
        Collider[] hitColliders = Physics.OverlapSphere(position, meleeRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.name);
            if (hitCollider.transform.GetComponentInParent<PlayerController>()) // get component in parent because im dumb
            {
                player.TakeDamage(meleeDamage);
                break;
            }
        }

        // reset sword rotation
        sword.rotation = initialRotation;

        if (!meleeCombo)
        {
            state = "chill";
            attackCooldown = meleeAttackingCooldown;
        }
        else if (meleeCombo && comboAtkNum > 4)
        {
            comboAtkNum = 0;
            state = "chill";
            attackCooldown = meleeAttackingCooldown;
        }
        else if (meleeCombo && comboAtkNum <= 4)
            StartCoroutine(Dash(true));
        else
            StartCoroutine(Retreat());
    }

    public IEnumerator Retreat()
    {
        if (stamina < dashStaminaCost)
        {
            state = "chill";
            yield break;
        }
        stamina -= dashStaminaCost;
        state = "dashing";
        attackTarget.position = new Vector3(-directionToPlayer.x * 20f, 0f, directionToPlayer.z * 20f);
        Vector3 dashDirection = directionToTarget;
        rb.velocity = Vector3.zero;
        transform.LookAt(new Vector3(attackTarget.position.x, position.y, attackTarget.position.z));
        yield return new WaitForSeconds(dashPauseTime);
        rb.velocity = dashDirection * dashSpeed;
        while (Vector3.Distance(transform.position, attackTarget.position) > 1f)
        {
            dashDirection = directionToTarget;
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        state = "chill";
    }

    public IEnumerator RangedAttack()
    {
        if (stamina < rangedStaminaCost)
        {
            state = "chill";
            yield break;
        }
        stamina -= rangedStaminaCost;
        state = "ranged";

        for (int i = 0; i < 100; i++)
        {
            UpdateTargetPosition();

            GameObject _projectile = Instantiate(projectile, firepoint.position, firepoint.rotation);
            BossProjectile projectileScript = _projectile.GetComponent<BossProjectile>();
            projectileScript.SetTarget(attackTarget.position);
            projectileScript.damage = rangedDamage;
            projectileScript.speed = rangedSpeed;

            yield return new WaitForSeconds(rangedCooldown);
        }

        state = "chill";
        attackCooldown = rangedAttackingCooldown;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangedDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeDistance);
    }
}
