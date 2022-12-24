using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private GameObject settings;
    [SerializeField] private CanvasGroup settingsBackground;
    [SerializeField] private RectTransform settingsWindow;
    [SerializeField] private Button closeButton;

    private bool _isOpen;
    
    // MonoBhevior Callbacks
    private void OnEnable()
    {
        InputSystem.OnOpenSettings += InputSystem_OnOpenSettings;
        closeButton.onClick.AddListener(CloseSettings);
    }

    private void OnDisable()
    {
        InputSystem.OnOpenSettings -= InputSystem_OnOpenSettings;
        closeButton.onClick.RemoveListener(CloseSettings);
    }
    
    // Private Methods ****
    private void InputSystem_OnOpenSettings(object sender, EventArgs e)
    {
        if(_isOpen) return;
        
        settings.SetActive(true);
        settingsBackground.alpha = 0;
        settingsWindow.localScale = Vector3.zero;

        LeanTween.alphaCanvas(settingsBackground, 1, 0.25f);
        LeanTween.scale(settingsWindow, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);

        _isOpen = true;
    }

    private void CloseSettings()
    {
        LeanTween.alphaCanvas(settingsBackground, 0, 0.5f);
        LeanTween.scale(settingsWindow, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            settings.SetActive(false);
            InputSystem.OnCloseSettings?.Invoke(this,EventArgs.Empty);
        });
        _isOpen = false;
    }
}
