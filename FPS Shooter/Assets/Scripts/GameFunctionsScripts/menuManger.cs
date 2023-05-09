using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class menuManger : MonoBehaviour
{
    //Componets and variables//
    public static menuManger instance;                     //The single instance of the gameManager singleton

    [Header("---------- Player Stuff ----------")]                    //Player game object and controller
    [SerializeField] public GameObject player;
    

    [Header("---------- UI Stuff ----------")]                        //UI menus and HUD elements
    [SerializeField] public GameObject activeMenu;
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject HTPMenu;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject winMenu;
    [SerializeField] public GameObject loseMenu;
    [SerializeField] public GameObject checkPoint;
    [SerializeField] public bool isPaused;



    [Header("----------Audio Stuff----------")]
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip backgroundMusic;
    [SerializeField] public AudioClip bossBattleMusic;



    float timeScaleOriginal;

    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;
        player = GameObject.FindGameObjectWithTag("Player");
       
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)    //Check for escape key press
        {
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }
}
