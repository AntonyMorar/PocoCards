using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public *****
    public static GameManager Instance { get; private set; }
    public bool DataLoaded { get; private set; }
    // Serialized
    [SerializeField] private DeckData defaultDeck;
    // Private ****
    private PlayerProfile _playerProfile;
    
    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        if(!DataLoaded) Load();
    }
    
    // Public Methods *****
    public void Save()
    {
        Debug.Log("Saving...");
        PlayerProfile playerProfile = new PlayerProfile(defaultDeck);
        string json = JsonUtility.ToJson(playerProfile);
        
        File.WriteAllText(Application.dataPath + "/save.json", json);
        DataLoaded = true;
    }

    public void Load()
    {
        Debug.Log("Loading...");
        string filePath = Application.dataPath + "/save.json";
        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            _playerProfile = JsonUtility.FromJson<PlayerProfile>(saveString);
            DataLoaded = true;
        }
        else
        {
            Debug.LogErrorFormat("No file found to load in ${0}", filePath);
        }
    }

    public PlayerProfile GetPlayerProfile() => _playerProfile;
    public Deck GetPlayerDeck() => _playerProfile.deck;
}
