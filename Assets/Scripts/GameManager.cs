using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Public *****
    public static GameManager Instance { get; private set; }

    public enum SceneState
    {
        Title = 1,
        MainMenu = 2,
        DeckEditor = 3,
        InGame = 4
    }

    public bool DataLoaded { get; private set; }

    // Serialized
    [SerializeField] private DeckData defaultDeck;

    [SerializeField] private DeckData availableCards;

    // Private ****
    private SceneState _state = SceneState.Title;
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

        if (!DataLoaded && !Load())
        {
            Save();
        }
    }

    // Public Methods *****
    public void SetState(SceneState state)
    {
        _state = state;
    }

    public SceneState GetState() => _state;

    public void Save()
    {
        Debug.Log("Saving...");
        PlayerProfile playerProfile = new PlayerProfile(defaultDeck);
        string json = JsonUtility.ToJson(playerProfile);

        File.WriteAllText(Application.dataPath + "/save.json", json);
        DataLoaded = true;
    }

    public bool Load()
    {
        Debug.Log("Loading...");
        string filePath = Application.dataPath + "/save.json";
        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            _playerProfile = JsonUtility.FromJson<PlayerProfile>(saveString);
            DataLoaded = true;
            return true;
        }
        else
        {
            Debug.LogErrorFormat("No file found to load in ${0}", filePath);
            return false;
        }
    }

    public PlayerProfile GetPlayerProfile() => _playerProfile;
}