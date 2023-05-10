using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerEverything : MonoBehaviour
{
    public AudioMixerGroup mixerGroup; // Set this in Inspector

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.outputAudioMixerGroup = mixerGroup;
        }
        else
        {
            Debug.Log("No AudioSource component found on this GameObject.");
        }
    }
}
