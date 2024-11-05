using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SillyCuber : MonoBehaviour
{
    public float rotationSpeed = 45;
    public bool idle = true;
    public bool moving;
    public Vector3 rotationVector;
    public Vector3 target;
    public Vector3 startPos;

    public PlayerController player;

    private void Awake()
    {
        startPos = transform.localPosition;
        target = transform.position;
        StartCoroutine(IdleRotations());
        player =  GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        transform.Rotate(rotationVector.normalized, rotationSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        if (idle)
        {
            StartCoroutine(Melee1());
        }
    }

    private IEnumerator Melee1()
    {
        StopCoroutine(IdleRotations());
        idle = false;
        // twirl
        rotationVector = new Vector3(-0.01f, 1, 0.01f);
        rotationSpeed = 360f;

        player.source.PlayOneShot(player.softAttackSound);

        // move forward
        float thingy = 0.0f;
        while (thingy < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0f, 0f, 4f), thingy / 0.1f);
            thingy += Time.deltaTime;
            yield return null;
        }

        player.godMode = true;
        yield return new WaitForSeconds(0.3f);
        player.godMode = false;
        // come back
        thingy = 0.0f;
        while (thingy < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, thingy / 0.1f);
            thingy += Time.deltaTime;
            yield return null;
        }
        idle = true;
        StartCoroutine(IdleRotations());
    }

    private IEnumerator IdleRotations()
    {
        if (idle)
        {
            rotationVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            rotationSpeed = Random.Range(-360, 360);
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(IdleRotations());
        StopCoroutine(IdleRotations());
    }
}
