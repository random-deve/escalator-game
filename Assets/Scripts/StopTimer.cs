using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTimer : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
        {
            gameManager.countTime = false;
        }
    }
}
