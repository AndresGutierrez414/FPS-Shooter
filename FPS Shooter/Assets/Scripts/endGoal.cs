using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endGoal : MonoBehaviour
{
    // variables //



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.activeMenu = gameManager.instance.winMenu;
            gameManager.instance.activeMenu.SetActive(true);
            gameManager.instance.pauseState();
        }
    }
}
