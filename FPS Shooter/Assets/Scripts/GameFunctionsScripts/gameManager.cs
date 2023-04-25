using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    //Componets and variables//
    public static gameManager instance;                     //The single instance of the gameManager singleton

    [Header("---------- Player Stuff ----------")]                    //Player game object and controller
    [SerializeField] public GameObject player;
    [SerializeField] public playerController playerScript;
    [SerializeField] public GameObject playerSpawnLocation;
    [SerializeField] public Image HPBar;
    [SerializeField] public Image SprintBar;

    [Header("---------- UI Stuff ----------")]                        //UI menus and HUD elements
    [SerializeField] public GameObject activeMenu;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject winMenu;
    [SerializeField] public GameObject loseMenu;
    [SerializeField] public GameObject checkPoint;
    [SerializeField] public bool isPaused;

    [Header("---------- Time Delayed Text ----------")]                        //UI menus and HUD elements
    [SerializeField] public TextMeshProUGUI endGoalText;
    public float endGoalTextDelayTimer;
    [SerializeField] public TextMeshProUGUI enemiesText;
    public float enemiesTextDelayTimer;
    [SerializeField] public TextMeshProUGUI weaponsText;
    public float weaponsTextDelayTimer;
    [SerializeField] public TextMeshProUGUI lavaText;
    public float lavaTextDelayTimer;

    [Header("----------Enemy Stuff----------")]
    public TextMeshProUGUI enemiesRemainingText;
    public int enemiesRemaining;

    [Header("----------Audio Stuff----------")]
    [SerializeField] public AudioSource backgroundMusic;

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

    private void Start()
    {
        playBackgroundMusic();

        // intro text display //
        endGoalText.gameObject.SetActive(false); // end goal
        StartCoroutine(endGoalTextFunction());
        enemiesText.gameObject.SetActive(false); // enemies
        StartCoroutine(enemiesTextFunction());
        weaponsText.gameObject.SetActive(false); // weapons 
        StartCoroutine(weaponsTextFunction());
        lavaText.gameObject.SetActive(false);    // lava
        StartCoroutine(lavaTextFunction());
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
        Cursor.visible= false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu= null;
    }

    public void playerDead()
    {
        pauseState();                                       //Set lose menu to active menu and pause the game
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    public void playBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    public void stopBackgroundMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
            backgroundMusic.Stop();
    }

    IEnumerator endGoalTextFunction()
    {
        yield return new WaitForSeconds(endGoalTextDelayTimer);
        endGoalText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        endGoalText.gameObject.SetActive(false);
    }

    IEnumerator enemiesTextFunction()
    {
        yield return new WaitForSeconds(enemiesTextDelayTimer);
        enemiesText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        enemiesText.gameObject.SetActive(false);
    }

    IEnumerator weaponsTextFunction()
    {
        yield return new WaitForSeconds(weaponsTextDelayTimer);
        weaponsText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        weaponsText.gameObject.SetActive(false);
    }

    IEnumerator lavaTextFunction()
    {
        yield return new WaitForSeconds(lavaTextDelayTimer);
        lavaText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        lavaText.gameObject.SetActive(false);
    }
}
