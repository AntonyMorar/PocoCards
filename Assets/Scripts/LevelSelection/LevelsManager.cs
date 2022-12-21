using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    // Public *****
    public static LevelsManager Instance { get; private set; }
    
    // Private *****


    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        
    }
    
    // Public Methods *****
    public void MoveToLevel()
    {
        SceneManager.LoadScene(3);
    }

    public void MoveToDeck()
    {
        SceneManager.LoadScene(2);
    }

    public void MoveToTitle()
    {
        SceneManager.LoadScene(0);
    }
}
