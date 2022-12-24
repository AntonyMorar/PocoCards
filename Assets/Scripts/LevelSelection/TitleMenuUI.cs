using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuUI : MonoBehaviour
{
    // Serialized
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        continueGameButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        newGameButton.onClick.RemoveListener(StartNewGame);
        continueGameButton.onClick.RemoveListener(ContinueGame);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    private void Start()
    {
        if (GameManager.Instance.IsSaved())
        {
            newGameButton.GetComponentInChildren<TMP_Text>().text = "Start New Game";
            continueGameButton.gameObject.SetActive(true);
        }
        else
        {
            newGameButton.GetComponentInChildren<TMP_Text>().text = "Start Game";
            continueGameButton.gameObject.SetActive(false);
        }
    }

    // Private Methods
    private void StartNewGame()
    {
        GameManager.Instance.Save();
        LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
    }

    private void ContinueGame()
    {
        GameManager.Instance.Load();
        LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
    }
    
    private void ExitGame()
    {
        Application.Quit();
    }
}
