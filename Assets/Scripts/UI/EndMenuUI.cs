using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndMenuUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private TMP_Text title;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

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
    }

    // Private Methods ****
    private void HideMenu()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.75f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    
    // Public Methods
    public void Set(bool won)
    {
        if (won)
        {
            title.text ="You Win!";
        }
        else
        {
            title.text = "You´re Dead";
        }
    }
}

