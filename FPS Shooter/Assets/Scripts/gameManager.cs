using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    //Componets and variables//
    public static gameManager instance;                     //The single instance of the gameManager singleton

    [Header("----- Player Stuff -----")]                    //Player game object and controller
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnLocation;

    [Header("----- UI Stuff -----")]                        //UI menus and HUD elements
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public bool isPaused;

    public Image HPBar;
    public TextMeshProUGUI enemiesRemainingText;
    public int enemiesRemaining;

    float timeScaleOriginal;

    //Awake() is called before Start(). Used to prevent accidental null reference
    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;
        player = GameObject.FindGameObjectWithTag("Player");
        playerSpawnLocation = GameObject.FindGameObjectWithTag("Spawn Location");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Updating");
        if (Input.GetButtonDown("Cancel") && activeMenu == null)    //Check for escape key press
        {
            Debug.Log("Escape key pressed");
            isPaused = !isPaused;                           //Toggle paused and set pause menu as active (or inactive)
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)                                   //Check for pause state
            {
                pauseState();
            }
            else
                unpauseState();
        }
    }

    public void pauseState()
    {
        Time.timeScale = 0;                                 //Za Waruldo! Toki wo tomare! Oh, and allows the cursor to move in the window
        Cursor.visible = true;                              /*The World! Time is stopped!*/
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unpauseState()
    {
        Time.timeScale = timeScaleOriginal;                 //Active window is deactivated, cursor is locked, and time is set back
        Cursor.visible= false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu= null;
    }

    //public void updateGameGoal(int amount)
    //{
    //    enemiesRemaining += amount;
    //    enemiesRemainingText.text = enemiesRemaining.ToString("F0"); // "F1" 1 float // "F0" int

    //    if (enemiesRemaining <= 0)                          //Check for no enemies remaining
    //    {
    //        activeMenu = winMenu;                           //Set win menu to active menu and pause the game
    //        activeMenu.SetActive(true);
    //        pauseState();
    //    }
    //}

    public void playerDead()
    {
        pauseState();                                       //Set lose menu to active menu and pause the game
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
}
