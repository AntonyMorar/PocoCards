using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    // Public *****
    public static LevelsManager Instance { get; private set; }
    public event EventHandler OnSceneLoad;
    public event EventHandler OnSceneLoaded;

    // Serialized **** 
    [SerializeField] private bool loadScene;
    [SerializeField] private GameManager.SceneState startLoadScene = GameManager.SceneState.Title;
    // Private *****
    private List<GameManager.SceneState> _loadedScenes = new List<GameManager.SceneState>();
    private bool _inTransition;
    

    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        
        if(loadScene) LoadScene(startLoadScene);
    }
    
    // Public Methods *****
    public void ChangeScene(GameManager.SceneState scene)
    {
        if (_loadedScenes.Contains(scene) || _inTransition ) return;

        StartCoroutine(ChangeSceneCorrutine(scene));
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

    private IEnumerator ChangeSceneCorrutine(GameManager.SceneState scene)
    {
        _inTransition = true;
        
        OnSceneLoad?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.666f);
        
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
        
        OnSceneLoaded?.Invoke(this,EventArgs.Empty);
        
        _inTransition = false;
    }
}
