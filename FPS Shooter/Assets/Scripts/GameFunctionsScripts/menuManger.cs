using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class menuManger : MonoBehaviour
{
    //Componets and variables//
    public static menuManger instance;                     //The single instance of the gameManager singleton

    [Header("---------- Player Stuff ----------")]                    //Have to have the player to stop the errors
    [SerializeField] public GameObject player;
    
    [Header("----------Audio Stuff----------")]             // Audio
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip backgroundMusic;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
       
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
