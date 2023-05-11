using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{

    public AudioSource audio;
    [SerializeField] RectTransform fader;
    [SerializeField] float delayTransition = 1f;

    private void Start()
    {
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1).setOnComplete(() =>
        {
            fader.gameObject.SetActive(false);
        });

    }

    // variables //
    //gameManager manager;
    public void gameStart()
    {
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1).setOnComplete(() =>
        {
            audio.Play();
            Invoke("MainSceneDelay", 1f);
            
        });
        
    }

    public void howToPlay()
    {
        
        gameManager.instance.mainMenu.SetActive(false);
        gameManager.instance.HTPMenu.SetActive(true);
        audio.Play();

    }

    public void back()
    {
        gameManager.instance.mainMenu.SetActive(true);
        gameManager.instance.HTPMenu.SetActive(false);
        audio.Play();

    }

    public void resume()
    {
        gameManager.instance.unpauseState();
        audio.Play();


        // toggle pause bool //
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }

    public void restart()
    {
        gameManager.instance.unpauseState();
        audio.Play();


        // reload the scene we are in //
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void quit()
    {
        audio.Play();
        Application.Quit();
    }

    public void MenuScene()
    {
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1).setOnComplete(() =>
        {
            audio.Play();
            Invoke("MenuSceneDelay", 1f);
            gameManager.instance.unpauseStateWithCursor();
            
        });
        

    }

    public void OptionsScene()
    {
        fader.gameObject.SetActive(true);
        //LeanTween.scale(fader, new Vector3(0, 0, 0), 1);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1).setOnComplete(() =>
        {
            audio.Play();
            Invoke("OptionsSceneDelay", 1f);
            
        });
        

    }

    public void CreditsScene()
    {
        fader.gameObject.SetActive(true);
        //LeanTween.scale(fader, new Vector3(0, 0, 0), 1);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, new Vector3(0, 0, 0), 1).setOnComplete(() =>
        {
            audio.Play();
            Invoke("CreditsSceneDelay", 1f);
        });
        
    }

    private void MainSceneDelay()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void CreditsSceneDelay()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    private void OptionsSceneDelay()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    private void MenuSceneDelay()
    {
        SceneManager.LoadScene("MenuScene");
    }



}
