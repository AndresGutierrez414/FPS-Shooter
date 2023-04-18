using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnLocation.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnLocation.transform.position = transform.position;
            StartCoroutine(flashColor());
        }
    }
    IEnumerator flashColor()
    {
        model.material.color = Color.green;
        gameManager.instance.checkPoint.SetActive(true);
        yield return new WaitForSeconds(.5f);
        model.material.color = Color.white;
        gameManager.instance.checkPoint.SetActive(false);
    }
}
