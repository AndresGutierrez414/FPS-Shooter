using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endGoal : MonoBehaviour
{
    public HighScore highScore;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            highScore.GameOver();
            gameManager.instance.activeMenu = gameManager.instance.winMenu;
            gameManager.instance.activeMenu.SetActive(true);
            gameManager.instance.playerScript.gunModel.gameObject.SetActive(false);
            gameManager.instance.playerScript.canShoot = false;
            gameManager.instance.pauseState();
        }
    }
}
