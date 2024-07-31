using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public GameManager gm;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>())
        {
            gm.ReloadScene();
        } else if (collision.gameObject.GetComponent<Enemy>())
        {
            collision.gameObject.transform.position = new Vector3(0, 50, 0);
        }
    }
}
