using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMenuUI : MonoBehaviour
{
    // Serializable
    [SerializeField] private EndMenuUI endMenu;

    // MonoBehaviour Callbacks
    private void OnEnable()
    {
        MatchManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDisable()
    {
        MatchManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }
    
    // Private Methods
    private void GameManager_OnGameOver(object sender, bool won)
    {
        endMenu.gameObject.SetActive(true);
        endMenu.Set(won);
    }
}
