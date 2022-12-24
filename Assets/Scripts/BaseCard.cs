using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseCard : MonoBehaviour
{
    [SerializeField] private Sprite faceSprite;
    [SerializeField] private Sprite backFaceSprite;
    // Serialized
    [Header("References")]
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    [SerializeField] private SpriteRenderer artAnchor;
    [SerializeField] private TMP_Text attackPointsText;
    // Private ****
    private CardData _cardData;
    
    // Public Methods **********
    public void SetCard(CardData cardData, bool flipped)
    {
        _cardData = cardData;
        
        // Visual
        mainSpriteRenderer.sprite = flipped ? faceSprite : backFaceSprite;
        artAnchor.sprite = null;
        attackPointsText.text = "";
    }

    public CardData GetCardData() => _cardData;
}
