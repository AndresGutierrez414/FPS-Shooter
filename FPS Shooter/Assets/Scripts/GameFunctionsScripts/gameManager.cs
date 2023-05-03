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
    [SerializeField] public GameObject cameraObject;
    [SerializeField] public cameraControls cameraScript;
    [SerializeField] public GameObject playerSpawnLocation;
    [SerializeField] public Image HPBar;
    [SerializeField] public Image SprintBar;

    [Header("---------- UI Stuff ----------")]                        //UI menus and HUD elements
    [SerializeField] public GameObject activeMenu;
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject HTPMenu;
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
    [SerializeField] public TextMeshProUGUI bossArrivalText;


    [Header("----------Enemy Stuff----------")]
    public TextMeshProUGUI enemiesRemainingText;
    public int enemiesRemaining;
    [SerializeField] public enemyAI bossEnemyScript;
    [SerializeField] public GameObject bossEnemy;

    [Header("----------Audio Stuff----------")]
    [SerializeField] public AudioSource backgroundMusic;
    [SerializeField] public AudioSource bossBattleMusic;


    float timeScaleOriginal;

    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;
        player = GameObject.FindGameObjectWithTag("Player");
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        bossEnemy = GameObject.FindGameObjectWithTag("Boss");
        playerSpawnLocation = GameObject.FindGameObjectWithTag("Spawn Location");
        playerScript = player.GetComponent<playerController>();
        cameraScript = cameraObject.GetComponent<cameraControls>();
        bossEnemyScript = bossEnemy.GetComponent<enemyAI>();
    }

    private void Start()
    {
        // play music //
        playBackgroundMusic();
        StartCoroutine(playBossBattleMusicAfterDelay(bossEnemyScript.riseDelay));

        // intro text display // 
        if (!cameraScript.enableIntroSequence)
        {
            StopAllCoroutines();
        }
        else
        {
            endGoalText.gameObject.SetActive(false);     // end goal
            StartCoroutine(endGoalTextFunction());
            enemiesText.gameObject.SetActive(false);     // enemies
            StartCoroutine(enemiesTextFunction());
            weaponsText.gameObject.SetActive(false);     // weapons 
            StartCoroutine(weaponsTextFunction());
            lavaText.gameObject.SetActive(false);        // lava
            StartCoroutine(lavaTextFunction());
            bossArrivalText.gameObject.SetActive(false); // enemy boss
            StartCoroutine(bossArrivalTextFunction());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the boss is destroyed and switch music
        if (bossEnemyScript.isBossDestroyed && !backgroundMusic.isPlaying) // Assuming 'isDead' is a boolean variable in the enemyAI script that is set to true when the boss is destroyed
        {
            stopBossBattleMusic();
            playBackgroundMusic();
        }

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

    public void playerDead()
    {
        pauseState();                                       //Set lose menu to active menu and pause the game
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    public void playBackgroundMusic()
    {
        if (backgroundMusic != null)
            backgroundMusic.Play();
    }

    public void stopBackgroundMusic()
    {
        if (backgroundMusic != null)
            backgroundMusic.Stop();
    }

    public void playBossBattleMusic()
    {
        if (bossBattleMusic != null)
            bossBattleMusic.Play();
    }

    public void stopBossBattleMusic()
    {
        if (bossBattleMusic != null)
            bossBattleMusic.Stop();
    }

    IEnumerator playBossBattleMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bossEnemy.activeInHierarchy)
        {
            playBossBattleMusic();
            stopBackgroundMusic();
        }
    }

    IEnumerator endGoalTextFunction()
    {
        yield return new WaitForSeconds(endGoalTextDelayTimer);
        if (!cameraScript.isSkippingIntro)
        {
            endGoalText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4);
            endGoalText.gameObject.SetActive(false);
        }
    }

    IEnumerator enemiesTextFunction()
    {
        yield return new WaitForSeconds(enemiesTextDelayTimer);
        if (!cameraScript.isSkippingIntro)
        {
            enemiesText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4);
            enemiesText.gameObject.SetActive(false);
        }
    }

    IEnumerator weaponsTextFunction()
    {
        yield return new WaitForSeconds(weaponsTextDelayTimer);
        if (!cameraScript.isSkippingIntro)
        {
            weaponsText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4);
            weaponsText.gameObject.SetActive(false);
        }
    }

    IEnumerator lavaTextFunction()
    {
        yield return new WaitForSeconds(lavaTextDelayTimer);
        if (!cameraScript.isSkippingIntro)
        {
            lavaText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4);
            lavaText.gameObject.SetActive(false);
        }
    }

    IEnumerator bossArrivalTextFunction()
    {
        yield return new WaitForSeconds(bossEnemyScript.riseDelay);
        if (!cameraScript.isSkippingIntro)
        {
            bossArrivalText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4);
            bossArrivalText.gameObject.SetActive(false);
        }
    }
}
