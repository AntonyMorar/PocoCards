using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardInfoUI : MonoBehaviour
{
    // Serialized
    [SerializeField] private TMP_Text cardInfoText;
    
    // MonoBehaviour Methods *****
    private void OnEnable()
    {
        GetComponent<RectTransform>().localScale = Vector3.zero;
        LeanTween.scale(GetComponent<RectTransform>(), Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    // Public Methods *****
    public void SetDescription(string description)
    {
        cardInfoText.text = description;
    }

    public void Hide()
    {
        LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, 0.1f).setEase(LeanTweenType.easeInBack)
            .setOnComplete(
                () =>
                {
                    gameObject.SetActive(false);
                });
    }
}
