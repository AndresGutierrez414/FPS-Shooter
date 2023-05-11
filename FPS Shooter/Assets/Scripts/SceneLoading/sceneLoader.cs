using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false; // Disable auto activation
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.fillAmount = progress;
            // Check if scene is loaded
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2f); // Add delay if you want

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
            // Error occurred while loading the scene
            Debug.LogError("Error loading scene: " + operation);
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float currentTime = 0f;
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
            yield return null;
        }
    }
}