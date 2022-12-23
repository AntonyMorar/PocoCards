using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    // Public *****
    public static LevelsManager Instance { get; private set; }

    // Serialized **** 
    [SerializeField] private GameManager.SceneState startScene;
    // Private *****
    private List<GameManager.SceneState> _loadedScenes = new List<GameManager.SceneState>();
    

    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        
        LoadScene(startScene);
    }

    // Private Methods *****
    private void LoadScene(GameManager.SceneState sceneIndex)
    {
        SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive);
        _loadedScenes.Add(sceneIndex); 
    }

    private void UnloadScene(GameManager.SceneState sceneIndex)
    {
        SceneManager.UnloadSceneAsync((int)sceneIndex);
        _loadedScenes.Remove(sceneIndex);
    }
    
    // Public Methods *****
    public void ChangeScene(GameManager.SceneState scene)
    {
        if (_loadedScenes.Contains(scene)) return;

        List<GameManager.SceneState> tempLoadedScenes = new List<GameManager.SceneState>();
        foreach (GameManager.SceneState loadedScene in _loadedScenes)
        {
            tempLoadedScenes.Add(loadedScene);
        }
        
        foreach (GameManager.SceneState loadedScene in tempLoadedScenes)
        {
            UnloadScene(loadedScene);
        }
        
        LoadScene(scene);
        GameManager.Instance.SetState(scene);
    }
}
