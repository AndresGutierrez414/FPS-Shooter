using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthAid : MonoBehaviour
{
    [SerializeField] int aid;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                player.reFillHealth(aid);
            }
            Destroy(gameObject);
        }
    }
}
