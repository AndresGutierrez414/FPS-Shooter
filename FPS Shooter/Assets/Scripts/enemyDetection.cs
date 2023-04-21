using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDetection : MonoBehaviour
{
    public enemyAI parentEnemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parentEnemyAI.playerEnteredRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parentEnemyAI.playerExitedRange();
        }
    }
}
