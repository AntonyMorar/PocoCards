using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
    // Serialized
    [SerializeField] private DeckData defaultDeck;
    [SerializeField] private DeckData availableCards;

    // Private ****
    private const string SAVE_PATH = "/save.json";
    private Camera _camera;
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
        
        _camera = Camera.main;

        if (IsSaved())
        {
            Load();
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
        if(_playerProfile == null) _playerProfile = new PlayerProfile(defaultDeck, availableCards);
        string json = JsonUtility.ToJson(_playerProfile);
        File.WriteAllText(Application.dataPath + SAVE_PATH, json);
    }
    
    public bool Load()
    {
        Debug.Log("Loading...");
        string filePath = Application.dataPath + SAVE_PATH;
        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            _playerProfile = JsonUtility.FromJson<PlayerProfile>(saveString);
            return true;
        }
        return false;
    }
    public bool IsSaved() => File.Exists(Application.dataPath + SAVE_PATH);
    public Camera GetCamera() => _camera;
    public PlayerProfile GetPlayerProfile() => _playerProfile;

    public void SetDecks(List<BaseCard> deck, List<BaseCard> collection)
    {
        _playerProfile.deck.Clear();
        foreach (BaseCard baseCard in deck)
        {
            _playerProfile.deck.Add(baseCard.GetCardData());
        }
        
        _playerProfile.collection.Clear();
        foreach (BaseCard baseCard in collection)
        {
            _playerProfile.collection.Add(baseCard.GetCardData());
        }
    }
}