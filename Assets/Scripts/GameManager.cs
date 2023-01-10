using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public event EventHandler OnTileMenuStart;
    public event EventHandler OnGameStart;

    // Serialized
    [SerializeField] private PlayerData playerData;
    [SerializeField] private DeckData availableCards;
    [Header("Enemies")] 
    [SerializeField] private List<PlayerData> enemies;
    // Private ****
    private const string SAVE_PATH = "/save.json";
    private Camera _camera;
    private SceneState _state = SceneState.Title;
    private PlayerProfile _playerProfile;
    private int _levelSelected = -1;


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

        switch (state)
        {
            case SceneState.Title:
            case SceneState.MainMenu: 
                OnTileMenuStart?.Invoke(this, EventArgs.Empty);
                break;
            case SceneState.InGame:
                OnGameStart?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public SceneState GetState() => _state;

    public void NewGame()
    {
        _playerProfile = null;
        Save();
    }
    public void Save()
    {
        if(_playerProfile == null) _playerProfile = new PlayerProfile(playerData, availableCards);
        string json = JsonUtility.ToJson(_playerProfile);
        File.WriteAllText(Application.dataPath + SAVE_PATH, json);
    }
    
    public bool Load()
    {
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
    public PlayerData GetPlayerData() => playerData;
    public List<CardData> GetPlayerDeck()
    {
        List<CardData> deck = new List<CardData>();

        foreach (PlayerProfile.PlayerCard playerCard in _playerProfile.allDeck)
        {
            if (!playerCard.unlocked) continue;
            if(playerCard.inDeck) deck.Add(playerCard.cardData);
        }

        return deck;
    }
    public PlayerData GetEnemy()
    {
        if (enemies.Count < _levelSelected ) return null;
 
        return enemies[_levelSelected];
    }

    /// <summary>
    /// Use this function when player wins a match
    /// </summary>
    public void UnlockLevel()
    {
        if(_playerProfile.levelsAvailable.Count <= _levelSelected + 1) return;
        
        _playerProfile.levelsAvailable[_levelSelected + 1] = true;
        Save();
    }

    public void SelectLevel(int level)
    {
        if (!_playerProfile.levelsAvailable[level]) return;
        _levelSelected = level;
    }

    public CardData UnlockRandomCard()
    {
        CardData randomLockedCard = _playerProfile.GetLockedRandomCard();

        foreach (PlayerProfile.PlayerCard playerCard in _playerProfile.allDeck)
        {
            if (playerCard.cardData == randomLockedCard)
            {
                playerCard.unlocked = true;
            }
        }

        return randomLockedCard;
    }

    public void ChangeDeck(CardData cardData, bool addToDeck)
    {
        foreach (PlayerProfile.PlayerCard playerCard in _playerProfile.allDeck)
        {
            if (playerCard.cardData == cardData)
            {
                playerCard.inDeck = addToDeck;
            }
        }
    }
    
}