using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoContainerUI : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private CardInfoUI cardInfoUI;
    
    // MonoBehavior Methods
    private void OnEnable()
    {
        Card.OnShowInfo += Card_OnShowInfo;
        Card.OnHideInfo += Card_OnHideInfo;
    }

    private void OnDisable()
    {
        Card.OnShowInfo -= Card_OnShowInfo;
        Card.OnHideInfo -= Card_OnHideInfo;
    }

    // Private Methods *****
    private void Card_OnShowInfo(object sender, string description)
    {
        cardInfoUI.gameObject.SetActive(true);
        cardInfoUI.SetDescription(description);
    }
    
    private void Card_OnHideInfo(object sender, EventArgs e)
    {
        cardInfoUI.Hide();
    }
}
