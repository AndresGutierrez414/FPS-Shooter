using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    // variables //

    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;

    [Header("----- UI Stuff -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;

    public int enemiesRemaining;

    public bool isPaused;
    float timeScaleOriginal;

    // awake called before start
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOriginal = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if escape key down //
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            // toggle bool //
            isPaused = !isPaused;
            activeMenu = pauseMenu;

            activeMenu.SetActive(isPaused);

            if (isPaused)
            {
                pauseState();
            }
            else
                unpauseState();
        }
    }

    public void pauseState()
    {
        // stops time in game //
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unpauseState()
    {
        // resume time to original state //
        Time.timeScale = timeScaleOriginal;
        Cursor.visible= false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu= null;
    }

    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;

        // if no more enemies, bring up win menu and pause game //
        if (enemiesRemaining <= 0)
        {
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            pauseState();
        }
    }
}
