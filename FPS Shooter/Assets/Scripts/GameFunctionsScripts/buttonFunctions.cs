using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public AudioSource audio;

    // variables //
    //gameManager manager;
    public void gameStart()
    {
        SceneManager.LoadScene("MainScene");
        audio.Play();
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
        Application.Quit();
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("MenuScene"); // goes to the Menu Scene
        audio.Play();

    }

    public void OptionsScene()
    {
        SceneManager.LoadScene("OptionsMenu"); // goes to Options Menu
        audio.Play();

    }

    public void CreditsScene()
    {
        SceneManager.LoadScene("CreditsScene"); // goes to Options Menu
        audio.Play();

    }
}
