using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject target;
    public Rigidbody rb;
    public float followPower;
    public float usedFollowPower;
    public int maxVelocity;
    public bool rigid;
    public bool looker;

    private float timer;
    private bool resetVelocity;

    private void Awake()
    {
        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
            rigid = true;
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            resetVelocity = false;
            timer = 5f;
        }
    }

    void FixedUpdate()
    {
        usedFollowPower = followPower;
        if (looker)
            transform.LookAt(target.transform);
        if (rigid)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (direction.y < 0)
                usedFollowPower *= 2;

            if (distance > 50 && !resetVelocity)
            {
                resetVelocity = true;
                if (direction.y > 0)
                    rb.velocity = Vector3.up * followPower;
                else
                    rb.velocity = Vector3.zero;
            }
            
            if (rb.velocity.magnitude >= maxVelocity) 
            {
                rb.velocity = Vector3.zero;
                return;
            }

            if (looker)
                rb.AddForce(transform.forward * (1f + Random.Range(0f, 1f)) * usedFollowPower * Mathf.Max(distance / 10f, 1), ForceMode.Impulse);
            else
                rb.AddForce(direction * (1f + Random.Range(0f, 1f)) * usedFollowPower * Mathf.Max(distance / 10f, 1), ForceMode.Impulse);

        }
    }
}
