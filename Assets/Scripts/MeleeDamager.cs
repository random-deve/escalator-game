using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamager : MonoBehaviour
{
    public Enemy stats;

    private void Awake()
    {
        if (GetComponent<Enemy>())
        {
            stats = GetComponent<Enemy>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(stats.damage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(stats.stayDamage * Time.deltaTime);
        }
    }
}
