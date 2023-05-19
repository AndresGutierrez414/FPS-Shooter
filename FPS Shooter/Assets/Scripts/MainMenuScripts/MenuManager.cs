using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> menuItems = new List<GameObject>(); 
    private int selectedIndex = 0;
    public buttonFunctions bfunctions;
    public AudioSource moveAudio;
     sceneLoader gM;

    void Start()
    {
        gM = GameObject.FindAnyObjectByType<sceneLoader>();
        SelectMenuItem(selectedIndex);
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            moveAudio.Play();
           DeselectMenuItem(selectedIndex);
            selectedIndex--;
            if (selectedIndex < 0) 
            {
                selectedIndex = menuItems.Count - 1;
            }
            SelectMenuItem(selectedIndex);
        }
       
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            moveAudio.Play();
            DeselectMenuItem(selectedIndex);
            selectedIndex++;
            if (selectedIndex >= menuItems.Count) 
            {
                selectedIndex = 0;
            }
            SelectMenuItem(selectedIndex);
        }

        else if (Input.GetKeyDown(KeyCode.Return) )
        {
            ActivateMenuItem(selectedIndex);
        }
    }
    void ActivateMenuItem(int index)
    {

       if ( menuItems[index].name == "Continue Game")
        {
            bfunctions.gameStart();
        }
        if (menuItems[index].name == "Settings")
        {
            gM.LoadSceneWithMouse(3);
        }
        if (menuItems[index].name == "New Game")
        {
            gM.LoadScene(1);
        }
       
        if (menuItems[index].name == "Quit")
        {
            bfunctions.quit();
        }
       if (menuItems[index].name == "Credits")
        {
            gM.LoadSceneWithMouse(4);
        }

        if(menuItems[index].name == "Resume")
        {
            bfunctions.resume();
        }
        if (menuItems[index].name == "Restart")
        {
            gM.ReloadScene();
        }
       
            if (menuItems[index].name == "HighScore")
        {
            gM.LoadSceneWithMouse(5);
        }
        if (menuItems[index].name == "Main Menu")
        {
            gM.LoadSceneWithMouse(0);
        }
        

    }
    void SelectMenuItem(int index)
    {
        
        
        menuItems[index].transform.GetChild(0).gameObject.SetActive(true);
    }

    void DeselectMenuItem(int index)
    {
        menuItems[index].transform.GetChild(0).gameObject.SetActive(false);
       
       
    }
}