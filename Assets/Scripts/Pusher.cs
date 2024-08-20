using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    public float mod;
    private Rigidbody rb;

    private PlayerController pc;
    private float moveTimer = 0f;
    private bool timerStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (moveTimer > 0f && timerStarted)
        {
            moveTimer -= Time.deltaTime;
        } else if (timerStarted)
        {
            pc.canMove = true;
            timerStarted = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.GetComponent<PlayerController>())
            return;
        pc.canMove = false;
        moveTimer = 0.1f;
        timerStarted = true;
        pc.transform.GetComponent<Rigidbody>().AddForce(new Vector3(rb.velocity.normalized.x, 0f, rb.velocity.normalized.z) * mod, ForceMode.Impulse);
    }
}
