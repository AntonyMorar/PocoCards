using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private string[] mainMenuList;
    [Header("References")] 
    [SerializeField] private Button backButton;
    [SerializeField] private Button mainButton;
    [SerializeField] private Button nextButton;
    
    // Private *****
    private TMP_Text _mainButtonText;
    private int _currentButtonIndex;
    
    // MonoBehaviour Callbacks
    private void OnEnable()
    {
        backButton.onClick.AddListener(GoBack);
        nextButton.onClick.AddListener(GoNext);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(GoBack);
        nextButton.onClick.RemoveListener(GoNext);
    }

    private void Start()
    {
        _mainButtonText = mainButton.GetComponent<TMP_Text>();
        if (!_mainButtonText) _mainButtonText = mainButton.GetComponentInChildren<TMP_Text>();

        _mainButtonText.text = mainMenuList[_currentButtonIndex];
        if (_currentButtonIndex == 0) backButton.gameObject.SetActive(false);

        SetMainButton(_currentButtonIndex);
    }

    // Private Methods *****
    private void GoBack()
    {
        if (_currentButtonIndex <= 0)return;
        _currentButtonIndex--;

        if (_currentButtonIndex < mainMenuList.Length - 1)
        {
            nextButton.gameObject.SetActive(true);
        }
        if (_currentButtonIndex == 0)
        {
            backButton.gameObject.SetActive(false);
        }
        
        // Setting MainButton
        SetMainButton(_currentButtonIndex);
        _mainButtonText.text = mainMenuList[_currentButtonIndex];
    }

    private void GoNext()
    {
        if (_currentButtonIndex >= mainMenuList.Length - 1)return;
        _currentButtonIndex++;

        if (_currentButtonIndex >= mainMenuList.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
        if (_currentButtonIndex > 0)
        {
            backButton.gameObject.SetActive(true);
        }
        
        // Setting MainButton
        SetMainButton(_currentButtonIndex);
        _mainButtonText.text = mainMenuList[_currentButtonIndex];
    }

    private void SetMainButton(int index)
    {
        mainButton.onClick.RemoveAllListeners();
        
        switch (index)
        {
            case 0:
                mainButton.onClick.AddListener(() =>
                {
                    LevelsManager.Instance.MoveToLevel();
                });
                break;
            case 1:
                mainButton.onClick.AddListener(() =>
                {
                    LevelsManager.Instance.MoveToDeck();
                });
                break;
            case 2:
                break;
            case 3:
                mainButton.onClick.AddListener(() =>
                {
                    LevelsManager.Instance.MoveToTitle();
                });
                break;
            default:
                break;
        }
    }
}
