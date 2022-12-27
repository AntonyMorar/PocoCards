using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerEffectUI : MonoBehaviour
{
    public enum Effect
    {
        Poisoned,
        Burned
    }
    
    // Serialized ******
    [SerializeField] private Image effectIcon;
    [SerializeField] private TMP_Text infoText;
    
    // private *****
    private string _effectName;
    
    // MonoBehavior Callbacks
    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        rectTransform.localScale = Vector3.zero;
        LeanTween.scale(rectTransform, Vector3.one, 0.25f).setEase(LeanTweenType.easeInQuad);
    }
    
    // Public Methods
    public void Set(Effect effect, int amount)
    {
        _effectName = GetEffectName(effect);
        infoText.text = _effectName + " " + amount;
    }

    public void UpdateAmount(int amount)
    {
        infoText.text = _effectName + " " + amount;
    }
    
    // Private Methods *****
    private string GetEffectName(Effect effect)
    {
        switch (effect)
        {
            case Effect.Poisoned:
                return "Poisoned";
            case Effect.Burned:
                return "Burned";
            default:
                return "N/E";
        }
    }

}
