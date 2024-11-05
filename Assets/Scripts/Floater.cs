using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 0.5f;
    public float rotationSpeed = 50f;
    public bool _float = true;
    public bool killOnPlayerContact = false;
    public AudioClip deathSound;

    public Vector3 rotateVector = new Vector3(0, 1, 0);
    public Vector3 floatOffset;
    private Vector3 realPos;

    void Start()
    {
        realPos = transform.position;
        floatHeight /= 600; // idk what happened but silly
    }

    void Update()
    {
        if (_float)
        {
            float newY = realPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(realPos.x, newY, realPos.z) + floatOffset;
            transform.Rotate(rotateVector.normalized, rotationSpeed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        realPos = transform.position - floatOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && killOnPlayerContact)
        {
            if (deathSound)
                GameObject.FindAnyObjectByType<AudioSource>().PlayOneShot(deathSound);
            Destroy(gameObject);
        }
    }
}
