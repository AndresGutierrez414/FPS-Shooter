using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoader : MonoBehaviour
{

    [SerializeField] public GameObject _sceneLoader;
    [SerializeField] public Image loadingBarRaw;

    public void SceneLoad(int sceneId)
    {
        StartCoroutine(SceneLoadAsync(sceneId));
    }
    IEnumerator SceneLoadAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        _sceneLoader.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBarRaw.fillAmount = progress;
            yield return null;
        }
    }
}
