using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public *****
    public static GameManager Instance { get; private set; }
    
    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
