using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioMixerGroup audioGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    public enum Music
    {
        Menu,
        Battle
    }
    public enum Sound
    {
        CardSlide,
        CardEffect,
        CardAttack,
        ItemObtain,
        
        UiChange,
        UiSelect,
        UiOpenClose,
        CardFlip,
        
        EffectCoin,
        EffectPoison,
        EffectFrozen,
        EffectHeal,
        CardEffectMiss
    }
    
    // MonoBehavior Callbackss
    private void Start()
    {
        PlayMusic(Music.Menu);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnTileMenuStart += GameManager_OnTileMenuStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTileMenuStart -= GameManager_OnTileMenuStart;
        GameManager.Instance.OnBattleStart -= GameManager_OnBattleStart;
    }

    // Public Methods ****
    public static void PlaySound(Sound sound, bool randomPitch = true)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
        //audioSource.outputAudioMixerGroup = audioGroup;
        if(randomPitch) audioSource.pitch = Random.Range(0.8f, 1.2f);
        
        Destroy(soundGameObject, GetAudioClip(sound).length + 0.1f);
    }
    
    // Private Methods ****
    private void GameManager_OnTileMenuStart(object sender, EventArgs e)
    {
        PlayMusic(Music.Menu);
    }
    
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        PlayMusic(Music.Battle);
    }
    private void PlayMusic(Music music)
    {
        if (musicAudioSource.clip == GetMusicClip(music)) return;
        
        musicAudioSource.clip = GetMusicClip(music);
        musicAudioSource.Play();
    }
    
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.I.soundAudioClips)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }

        return null;
    }
    
    private static AudioClip GetMusicClip(Music music)
    {
        foreach (GameAssets.SoundMusicClip soundAudioClip in GameAssets.I.musicAudioClips)
        {
            if (soundAudioClip.music == music) return soundAudioClip.audioClip;
        }
        return null;
    }
}
