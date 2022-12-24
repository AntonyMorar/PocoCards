using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatchMenuUI : MonoBehaviour
{
    // Serializable
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private VictoryUI victoryUI;
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
        if (won) victoryUI.gameObject.SetActive(true); 
        else gameOverUI.gameObject.SetActive(true);
    }

    private void OnOpenSettings()
    {
        InputSystem.OnOpenSettings?.Invoke(this,EventArgs.Empty);
    }
}
