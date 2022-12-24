using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    // Serialized
    [SerializeField] private Button exitButton;
    
    // MonoBehaviour Callbacks
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.75f).setEase(LeanTweenType.easeOutBack);
        
        exitButton.onClick.AddListener(() =>
        {
            LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
            HideMenu();
        });
    }
    
    private void HideMenu()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.75f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
