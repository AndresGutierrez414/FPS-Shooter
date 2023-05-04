using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    // variables //
    //gameManager manager;
    public void gameStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void howToPlay()
    {
        gameManager.instance.mainMenu.SetActive(false);
        gameManager.instance.HTPMenu.SetActive(true);
    }

    public void back()
    {
        gameManager.instance.mainMenu.SetActive(true);
        gameManager.instance.HTPMenu.SetActive(false);
    }

    public void resume()
    {
        gameManager.instance.unpauseState();

        // toggle pause bool //
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }

    public void restart()
    {
        gameManager.instance.unpauseState();

        // reload the scene we are in //
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
