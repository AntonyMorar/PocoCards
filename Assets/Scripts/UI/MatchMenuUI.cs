using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMenuUI : MonoBehaviour
{
    // Serializable
    [SerializeField] private EndMenuUI endMenu;
    [SerializeField] private Button pauseButton;

    // MonoBehaviour Callbacks
    private void OnEnable()
    {
        MatchManager.Instance.OnGameOver += GameManager_OnGameOver;
        pauseButton.onClick.AddListener(OnOpenSettings);
    }

    private void OnDisable()
    {
        MatchManager.Instance.OnGameOver -= GameManager_OnGameOver;
        pauseButton.onClick.RemoveListener(OnOpenSettings);
    }
    
    // Private Methods
    private void GameManager_OnGameOver(object sender, bool won)
    {
        endMenu.gameObject.SetActive(true);
        endMenu.Set(won);
    }

    private void OnOpenSettings()
    {
        InputSystem.OnOpenSettings?.Invoke(this,EventArgs.Empty);
    }
}
