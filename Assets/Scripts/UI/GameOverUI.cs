using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartButton;
    
    // MonoBehavior Callbacks *****
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.75f).setEase(LeanTweenType.easeOutBack);

        restartButton.onClick.AddListener(() =>
        {
            MatchManager.Instance.RestartGame();
            HideMenu();
        });
        
        exitButton.onClick.AddListener(() =>
        {
            LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
        });
    }

    // Private Methods ****
    private void HideMenu()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.75f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}

