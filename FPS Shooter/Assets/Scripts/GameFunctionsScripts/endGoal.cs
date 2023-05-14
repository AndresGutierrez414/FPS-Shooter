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
            gameManager.instance.pauseState();
        }
    }
}
