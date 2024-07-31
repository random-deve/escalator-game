using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptancePillar : MonoBehaviour
{
    public Enemy boss;
    private Enemy enemySelf;

    private void Awake()
    {
        enemySelf = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemySelf.dead && gameObject.activeInHierarchy)
        {
            Debug.Log("war");
            boss.gameObject.GetComponent<Enemy>().TakeDamage(250f);
            gameObject.SetActive(false);
        }
    }
}
