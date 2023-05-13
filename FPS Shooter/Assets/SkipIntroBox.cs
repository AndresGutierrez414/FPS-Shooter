using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipIntroBox : MonoBehaviour
{
    public GameObject walls;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(walls);
        }
    }
}
