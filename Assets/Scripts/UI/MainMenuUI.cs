using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    // Serialized
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayGame);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    // Private Methods
    private void PlayGame()
    {
        SceneManager.LoadScene(2);
    }
    
    private void ExitGame()
    {
        Application.Quit();
    }
}
