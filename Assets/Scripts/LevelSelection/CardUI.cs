using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private Image artImage;
    [SerializeField] private TMP_Text attackPointsText;
    // Private ****
    private CardData _cardData;
    
    // Public Methods **********
    public void SetCard(CardData cardData)
    {
        _cardData = cardData;
        // Visual
        artImage.sprite = cardData.cardIcon;
        attackPointsText.text = cardData.attackPoints.ToString();
    }

    public CardData GetCardData() => _cardData;
}
