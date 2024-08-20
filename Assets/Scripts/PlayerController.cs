using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Game")]
    public GameManager gameManager;
    public Volume volume;

    public float damageVignette;
    public float damageVignetteTime;
    public bool damageVignetteActive;

    public float maxHealth;
    public float health;
    public HealthBar healthBar;
    public bool godMode;

    public float passiveHealRate;
    public bool saturationScaling = true;

    public SillyCuber magicCube;

    [Header("Movement")]
    public GameObject playerCam;
    public CameraShake shake;

    public bool canMove = true;

    private float initMoveSpeed;
    public float moveSpeed;
    private float initInitAirMove;
    public float airMoveSpeed;
    public bool slowing;
    public float slowingTimer;
    public float minMoveSpeed;
    public float timeTillMinSpeed;
    private float friction;
    public float normFriction;
    public float jumpForce;
    private float initJumpForce;
    public float jumpCD;
    public float drag;
    public float jumpMod;
    public bool canJump = true;

    public float momentumPreservationTime;
    private bool preserveMomentum;
    private float momentumTimer;

    public float maxSlopeAngle;
    public RaycastHit slopeHit;

    private float initAirMove;
    private float gravMod;
    private float initGravMod;

    [Header("Spells")]
    public bool canCast = true;

    public float spell1Cooldown;
    // public float spell2Cooldown;

    private float spell1Timer;
    // private float spell2Timer;

    public GameObject spell1Obj;
    // public GameObject spell2Obj;
    private GameObject currentSpellObj;

    public float maxCastDistance;
    public LayerMask castLayers;

    [Header("Grappling")]
    public float grappleSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float maxGrappleDistance;
    public float grappleTimer;
    public float grappleMaxTime;
    public float grappleShakeDuration;
    public float grappleShakeX;
    public float grappleShakeY;
    public LayerMask grappleLayer;
    public bool canGrapple = true;
    public bool wholeSceneCanGrapple = true;
    public bool landToRefresh;

    public Vector3 grapplePoint;
    private float initGrappleSpeed;
    public bool isGrappling;

    public GameObject grappleCoordinateObj;
    public GameObject grappleCoordinateDeathEffect;
    public GameObject grappleCoordinateStartEffect;
    private GameObject currentGrappleObj;

    [Header("Keybinds")]
    public KeyCode meleeBtn = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode grappleKey = KeyCode.G;
    public KeyCode spell1Key;
    // public KeyCode spell2Key;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    public bool grounded;
    public bool walling;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public enum MovementState
    {
        moving,
        air
    }

    public MovementState state;

    private void Awake()
    {
        health = maxHealth;
        healthBar.maxHealth = maxHealth;
        healthBar.health = health;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        canJump = true;
        initMoveSpeed = moveSpeed;
        initAirMove = airMoveSpeed;
        initInitAirMove = initAirMove;
        initGravMod = gravMod;
        initGrappleSpeed = grappleSpeed;
        initJumpForce = jumpForce;
        if (!gameManager.countTime)
            gameManager.countTime = true;
        if (playerCam.GetComponent<CameraShake>())
        {
            shake = playerCam.GetComponent<CameraShake>();
        }
        volume = gameManager.volume;
        magicCube = GameObject.Find("Magic Cube").GetComponent<SillyCuber>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        } else if (passiveHealRate > 0f && health < maxHealth)
        {
            // TakeDamage(-passiveHealRate * Time.deltaTime);
        }

        if (slowing)
        {
            slowingTimer += Time.deltaTime / timeTillMinSpeed;

            slowingTimer = Mathf.Clamp01(slowingTimer);

            moveSpeed = Mathf.Lerp(initMoveSpeed, minMoveSpeed, slowingTimer);
            initAirMove = Mathf.Lerp(initInitAirMove, minMoveSpeed, slowingTimer);
            jumpForce = Mathf.Lerp(initJumpForce, 0.75f * initJumpForce, slowingTimer);

            if (moveSpeed <= minMoveSpeed)
            {
                slowing = false;
            }
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        HandleState();
        GetInput();
        if (grappleTimer > 0)
        {
            grappleTimer -= Time.deltaTime;
        }
        else if (isGrappling)
        {
            EndGrapple();
        }

        if (preserveMomentum)
        {
            momentumTimer -= Time.deltaTime;
            if (momentumTimer <= 0)
                preserveMomentum = false;
        }

        if (grounded && isGrappling)
        {
            preserveMomentum = true;
            momentumTimer = momentumPreservationTime;
        }

        if (spell1Timer > 0)
        {
            spell1Timer -= Time.deltaTime;
        }
        /*if (spell2Timer > 0)
        {
            spell2Timer -= Time.deltaTime;
        }*/

        if (Input.GetKeyDown(meleeBtn))
        {
            Melee();
        }
    }

    public void Melee()
    {
        magicCube.Attack();
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    public void StartGrapple()
    {
        if (!wholeSceneCanGrapple)
            return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, maxGrappleDistance, grappleLayer))
        {
            if (isGrappling)
                EndGrapple();
            gravMod = initGravMod;
            grapplePoint = hit.point;
            isGrappling = true;
            rb.useGravity = false;
            currentGrappleObj = Instantiate(grappleCoordinateObj, grapplePoint, Quaternion.identity);
            if (grappleCoordinateStartEffect)
                Instantiate(grappleCoordinateStartEffect, grapplePoint, Quaternion.Euler(0, 0, 0));
            // canGrapple = false;
            grappleTimer = grappleMaxTime;
        }
        else if (isGrappling)
        {
            EndGrapple();
        }
    }

    public void GrappleMove()
    {
        if (currentGrappleObj == null)
        {
            EndGrapple();
            return;
        }

        grapplePoint = currentGrappleObj.transform.position;
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);
        grappleSpeed = Mathf.Lerp(minSpeed, maxSpeed, maxGrappleDistance / distance);

        rb.velocity = direction * grappleSpeed;

        if (distance < 3f && currentGrappleObj.GetComponent<Floater>().killOnPlayerContact)
        {
            EndGrapple();
            shake.Shake(grappleShakeDuration, grappleShakeX, grappleShakeY);
        }
    }

    public void EndGrapple()
    {
        isGrappling = false;
        grappleSpeed = initGrappleSpeed;
        rb.useGravity = true;
        Destroy(currentGrappleObj);
        if (!landToRefresh)
        {
            canGrapple = true;
        }
    }

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            GrappleMove();
        }
        else if (canMove)
        {
            MovePlayer();
        }
    }

    public void TakeDamage(float _damage)
    {
        if (godMode && _damage > 0f)
            _damage = 0f;
        health -= _damage;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.SetHealth(health);
        if (!damageVignetteActive && _damage > 0f)
            StartCoroutine(TakeDamageEffect());
        if (volume.profile.TryGet(out ColorAdjustments adjustments) && saturationScaling)
        {
            adjustments.saturation.value = -Mathf.Lerp(100, 25, health/maxHealth);
        }
    }

    private IEnumerator TakeDamageEffect()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            damageVignetteActive = true;

            float elapsed = 0f;
            float intensity = damageVignette;
            vignette.color.value = Color.red;

            while (elapsed < damageVignetteTime)
            {
                if (elapsed >= damageVignette / 2)
                    intensity = Mathf.Lerp(intensity, 0, elapsed / damageVignetteTime*2 + 0.5f);
                vignette.intensity.value = intensity;
                elapsed += Time.deltaTime;
                yield return null;
            }

            vignette.intensity.value = 0f;
            damageVignetteActive = false;
        }
    }

    public void Die()
    {
        // change to something better later
        gameManager.ReloadScene();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCD);
        }

        if (Input.GetKeyDown(grappleKey) && canGrapple)
        {
            if (!isGrappling)
                StartGrapple();
            else
            {
                EndGrapple();
                Instantiate(grappleCoordinateDeathEffect, grapplePoint, Quaternion.Euler(0, 0, 0));
                if (!preserveMomentum)
                {
                    momentumTimer = momentumPreservationTime;
                    preserveMomentum = true;
                }
            }
        }
        
        if (spell1Timer <= 0 && Input.GetKeyDown(spell1Key) && canCast)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, maxCastDistance, castLayers))
            {
                currentSpellObj = Instantiate(spell1Obj, hit.point, Quaternion.identity);
                spell1Timer = spell1Cooldown;
            }
        }
        /*if (spell2Timer <= 0 && Input.GetKeyDown(spell2Key) && canCast)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, maxCastDistance, castLayers))
            {
                currentSpellObj = Instantiate(spell2Obj, hit.point, Quaternion.identity);
                spell2Timer = spell2Cooldown;
            }
        }*/
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void HandleState()
    {
        if (!grounded)
        {
            state = MovementState.air;
        }
        else
        {
            state = MovementState.moving;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.useGravity = !OnSlope();

        if (OnSlope())
        {
            rb.velocity = new Vector3((GetSlopeMoveDirection() * moveSpeed).x, rb.velocity.y, (GetSlopeMoveDirection() * moveSpeed).z);
            return;
        }

        if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, 1f, ground))
        {
            walling = true;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
        }
        else
        {
            walling = false;
        }

        if (state != MovementState.air)
        {
            if (preserveMomentum)
            {
                Vector3 currentVelocity = rb.velocity;
                Vector3 inputVelocity = moveDirection * 2 * moveSpeed;

                rb.velocity = Vector3.Lerp(currentVelocity, inputVelocity, Time.deltaTime * momentumPreservationTime * 2f);
            }
            else
            {
                rb.velocity = new Vector3((moveDirection * moveSpeed).x, rb.velocity.y, (moveDirection * moveSpeed).z);
                airMoveSpeed = initAirMove;
                gravMod = initGravMod;
                canGrapple = true;
            }
        }
        else if (state == MovementState.air)
        {
            if (!walling && !preserveMomentum)
            {
                rb.velocity = new Vector3((moveDirection * airMoveSpeed).x, rb.velocity.y + gravMod, (moveDirection * airMoveSpeed).z);
                airMoveSpeed *= 1 + drag * Time.deltaTime;
            }
            else if (!walling && preserveMomentum)
            {
                Vector3 currentVelocity = rb.velocity;
                Vector3 inputVelocity = moveDirection * 2 * airMoveSpeed;
                rb.velocity = Vector3.Lerp(currentVelocity, inputVelocity, Time.deltaTime * momentumPreservationTime * 2f);
            }

            gravMod -= jumpMod * Time.deltaTime;
        }
    }
}
