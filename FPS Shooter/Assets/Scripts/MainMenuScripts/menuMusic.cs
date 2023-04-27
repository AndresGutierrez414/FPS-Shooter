using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuMusic : MonoBehaviour
{
    // variables //
    [SerializeField] AudioSource startMenuMusic;


    // Start is called before the first frame update
    void Start()
    {
        startMenuMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
