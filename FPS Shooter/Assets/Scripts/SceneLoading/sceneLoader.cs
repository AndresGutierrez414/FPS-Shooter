using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject UIScreen;
    [SerializeField] private Image loadingBar;

    public void LoadSceneWithMouse(int sceneId)
    {
        StartCoroutine(LoadSceneAsyncwithMouse(sceneId));
    }
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }
    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneAsync());
    }
    private IEnumerator LoadSceneAsyncwithMouse(int sceneId)
    {
        loadingBar.fillAmount = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        operation.allowSceneActivation = false; // Disable auto activation

        UIScreen.SetActive(false);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.fillAmount = progress;

            loadingText.SetText(Mathf.Round(progress * 100f) + "%");

            // Check if scene is loaded
            if (operation.progress >= 0.9f)
            {
                gameManager.instance.unpauseStateWithCursor();
                yield return new WaitForSeconds(1f);

                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        loadingScreen.SetActive(false);

        // Add error handling
        if (operation.isDone && operation.allowSceneActivation)
        {
            // Scene loaded successfully
            Debug.Log("Scene loaded successfully");
        }
        else
        {
            // Error while loadin the scene
            Debug.LogError("Error loading scene: " + operation);
        }
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        loadingBar.fillAmount = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        operation.allowSceneActivation = false; // Disable auto activation

        UIScreen.SetActive(false);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.fillAmount = progress;

            loadingText.SetText(Mathf.Round(progress * 100f) + "%");

            // Check if scene is loaded
            if (operation.progress >= 0.9f)
            {
                gameManager.instance.unpauseState();
                yield return new WaitForSeconds(1f);

                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        loadingScreen.SetActive(false);

        // Add error handling
        if (operation.isDone && operation.allowSceneActivation)
        {
            // Scene loaded successfully
            Debug.Log("Scene loaded successfully");
        }
        else
        {
            // Error while loadin the scene
            Debug.LogError("Error loading scene: " + operation);
        }
    }
    private IEnumerator ReloadSceneAsync()
    {
        loadingBar.fillAmount = 0f;

       
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        operation.allowSceneActivation = false; // Disable auto activation

        UIScreen.SetActive(false);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.fillAmount = progress;

            // Check if scene is loaded
            if (operation.progress >= 0.9f)
            {
                
                gameManager.instance.unpauseState();
                yield return new WaitForSeconds(1f);
               
                operation.allowSceneActivation = true; // Allow scene activation
               
            }
           
            yield return null;
            
        }
        
        loadingScreen.SetActive(false);

        // Add error handling
        if (operation.isDone && operation.allowSceneActivation)
        {
            // Scene loaded successfully
            Debug.Log("Scene reloaded successfully");
        }
        else
        {
            // Error while loadin the scene
            Debug.LogError("Error reloading scene: " + operation);
        }
    }
}