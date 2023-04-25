using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeathAid : MonoBehaviour
{
    [SerializeField] int healthAid;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if(player != null)
            {
                player.reFillHealth();
            }
            Destroy(gameObject);
        }
    }
}
