using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerPatroller : MonoBehaviour
{
    public GameObject target;
    public Rigidbody rb;
    public float followPower;
    public float usedFollowPower;
    public bool rigid;
    public bool looker;
    public bool drawRange;

    public float patrolRange = 20f;
    public float followDistance = 15f;
    public float patrolSpeed = 5f;
    private Vector3 patrolCenter;
    private Vector3 patrolTarget;

    private float timer;
    private bool resetVelocity;

    public bool patrolBounds;
    public Vector3 patrolBoundsMin;
    public Vector3 patrolBoundsMax;

    private void Awake()
    {
        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
            rigid = true;
        }

        patrolCenter = transform.position;
        SetNewPatrolTarget();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
            resetVelocity = false;
            timer = Random.Range(3f, 7f);
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= followDistance)
        {
            FollowTarget();
        }
        else
        {
            Patrol();
        }
    }

    void FixedUpdate()
    {
        usedFollowPower = followPower;
        if (looker)
            transform.LookAt(target.transform);
    }

    private void FollowTarget()
    {
        if (rigid)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (direction.y < 0)
                usedFollowPower *= 2;

            if (distance > 50 && !resetVelocity)
            {
                resetVelocity = true;
                rb.velocity = Vector3.zero;
            }
            if (looker)
                rb.AddForce(transform.forward * (1f + Random.Range(0f, 1f)) * usedFollowPower * Mathf.Max(distance / 10f, 1), ForceMode.Impulse);
            else
                rb.AddForce(direction * (1f + Random.Range(0f, 1f)) * usedFollowPower * Mathf.Max(distance / 10f, 1), ForceMode.Impulse);
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolTarget) < 2f)
        {
            SetNewPatrolTarget();
        }

        Vector3 direction = (patrolTarget - transform.position).normalized;
        rb.AddForce(direction * patrolSpeed, ForceMode.Force);
    }

    private void SetNewPatrolTarget()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
        patrolTarget = patrolCenter + randomOffset;

        if (patrolBounds)
            patrolTarget = new Vector3(
                Mathf.Clamp(patrolTarget.x, patrolBoundsMin.x, patrolBoundsMax.x),
                Mathf.Clamp(patrolTarget.y, patrolBoundsMin.y, patrolBoundsMax.y),
                Mathf.Clamp(patrolTarget.z, patrolBoundsMin.z, patrolBoundsMax.z)
            );

        rb.velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (drawRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(patrolCenter, patrolRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, followDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(patrolTarget, 0.5f);
        }

        if (patrolBounds)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(patrolBoundsMin.x, patrolBoundsMin.y, patrolBoundsMin.z), new Vector3(patrolBoundsMax.x, patrolBoundsMin.y, patrolBoundsMin.z));
            Gizmos.DrawLine(new Vector3(patrolBoundsMin.x, patrolBoundsMin.y, patrolBoundsMin.z), new Vector3(patrolBoundsMin.x, patrolBoundsMax.y, patrolBoundsMax.z));
            Gizmos.DrawLine(new Vector3(patrolBoundsMax.x, patrolBoundsMax.y, patrolBoundsMax.z), new Vector3(patrolBoundsMax.x, patrolBoundsMin.y, patrolBoundsMin.z));
            Gizmos.DrawLine(new Vector3(patrolBoundsMax.x, patrolBoundsMax.y, patrolBoundsMax.z), new Vector3(patrolBoundsMin.x, patrolBoundsMax.y, patrolBoundsMax.z));
        }
    }
}
