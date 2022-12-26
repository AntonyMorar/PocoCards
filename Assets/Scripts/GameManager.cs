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
    [SerializeField] private PlayerData playerData;
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
    }

    public SceneState GetState() => _state;
    
    public void Save()
    {
        if(_playerProfile == null) _playerProfile = new PlayerProfile(playerData);
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
        _playerProfile.levelCompleted++;
        Save();
    }

    public void SelectLevel(int level)
    {
        if (level > _playerProfile.levelCompleted) return;
        
        _levelSelected = level;
    }
}