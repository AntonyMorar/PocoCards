using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    // Public *****
    public static LevelsManager Instance { get; private set; }
    
    // Private *****
    private int _nextSceneToLoad;

    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        _nextSceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }
    
    // Public Methods *****
    public void MoveToNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            
            return;
        }
        
        SceneManager.LoadScene(_nextSceneToLoad);

        if (_nextSceneToLoad > PlayerPrefs.GetInt("levelAt"))
        {
            PlayerPrefs.SetInt("levelAt", _nextSceneToLoad);
        }
    }

    public void MoveToTitle()
    {
        SceneManager.LoadScene(0);
        _nextSceneToLoad = 1;
    }
}
