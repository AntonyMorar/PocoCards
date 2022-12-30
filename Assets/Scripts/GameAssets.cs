using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets I { get; private set; }

    private void Awake()
    {
        if (I != null && I != this) Destroy(this);
        else
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public SoundMusicClip[] musicAudioClips;
    public SoundAudioClip[] soundAudioClips;
    
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
    
    [System.Serializable]
    public class SoundMusicClip
    {
        public SoundManager.Music music;
        public AudioClip audioClip;
    }
}
