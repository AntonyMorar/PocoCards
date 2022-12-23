using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    // MonoBehavior Callbacks
    private void Start()
    {
        foreach (CardData cardData in GameManager.Instance.GetPlayerProfile().deck)
        {
            Debug.Log(cardData.cardName);
        }
    }
}
