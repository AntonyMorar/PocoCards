using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    // Public ****
    public static AudioPlayer Instance;
    // Serialized ****

    // Private ****
    private AudioSource _audioSource;
    
    // MonoBehaviour Callbacks
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }
}
