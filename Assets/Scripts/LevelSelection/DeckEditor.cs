using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private Hand playerDeck;
    [SerializeField] private Hand playerCollection;
    
    // MonoBehavior Callbacks ****
    private void Start()
    {
        GameManager.Instance.SetState(GameManager.SceneState.DeckEditor);
        
        foreach (CardData cardData in GameManager.Instance.GetPlayerProfile().deck)
        {
            playerDeck.AddCard(cardData);
        }
        
        foreach (CardData cardData in GameManager.Instance.GetPlayerProfile().collection)
        {
            if(!GameManager.Instance.GetPlayerProfile().deck.Contains(cardData))
                playerCollection.AddCard(cardData);
        }
    }

    private void OnEnable()
    {
        InputSystem.OnCardClick += InputSystem_OnCardClick;
    }

    private void OnDisable()
    {
        InputSystem.OnCardClick += InputSystem_OnCardClick;
    }
    
    // Private methods **** 
    private void InputSystem_OnCardClick(object sender, BaseCard baseCard)
    {
        if (playerDeck.GetCards().Contains(baseCard))
        {
            AddToCollection(baseCard);
        }
        else if (playerCollection.GetCards().Contains(baseCard))
        {
            AddToDeck(baseCard);
        }
    }

    private void AddToCollection(BaseCard baseCard)
    {
        playerCollection.AddCard(baseCard.GetCardData());
        playerDeck.RemoveCard(baseCard);
        
        GameManager.Instance.SetDecks(playerDeck.GetCards(), playerCollection.GetCards());
    }

    private void AddToDeck(BaseCard baseCard)
    {
        playerDeck.AddCard(baseCard.GetCardData());
        playerCollection.RemoveCard(baseCard);
        
        GameManager.Instance.SetDecks(playerDeck.GetCards(), playerCollection.GetCards());
    }
}
