using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene("LoadingScene");
        StartCoroutine(LoadTargetSceneAsync(sceneName));
    }

    private IEnumerator LoadTargetSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Here you can update your loading screen with progress information
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + progress);
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Add an artificial delay of 1 second

        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}
