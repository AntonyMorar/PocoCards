using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    // Public *****
    public static InputSystem Instance { get; private set; }
    
    public static EventHandler OnOpenSettings;
    public static EventHandler OnCloseSettings;


    // MonoBehavior Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Update()
    {
        OnScapeDown();
    }

    private void OnScapeDown()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if(GameManager.Instance.GetState() == GameManager.SceneState.DeckEditor)
            LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
    }

}
