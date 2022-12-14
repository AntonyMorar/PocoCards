using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusUI : MonoBehaviour
{
    // Serializable
    [SerializeField] private GameObject endMenu;

    // Monobehaviour Callbacks
    private void OnEnable()
    {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }
    
    // Private Methods
    private void GameManager_OnGameOver(object sender, EventArgs e)
    {
        endMenu.SetActive(true);
    }
}
