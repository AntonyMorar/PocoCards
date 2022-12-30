using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    // Serialized
    [SerializeField] private Button exitButton;

    [Header("Unlocked Card")] 
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text description;
    
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
    
    // Public Methods ****
    public void UnlockCard(CardData cardData)
    {
        Debug.Log("Unlock new card: " + cardData.cardName);
        cardImage.sprite = cardData.cardIcon;
        cardName.text = cardData.cardName;
        description.text = cardData.description;
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
