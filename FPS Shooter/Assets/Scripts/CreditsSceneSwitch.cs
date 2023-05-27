using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsSceneSwitch : MonoBehaviour
{

    [SerializeField] int seconds;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadSceneAfterDelay(seconds));
    }

    IEnumerator loadSceneAfterDelay(float seconds2)
    {
        seconds2 = seconds;
        yield return new WaitForSeconds(seconds2);
        SceneManager.LoadScene("MenuScene");
    }

}
