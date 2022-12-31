using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    private int MAX_DECK = 6;
    // Serialized *****
    [SerializeField] private Hand playerDeck;
    [SerializeField] private Hand playerCollection;
    
    // MonoBehavior Callbacks ****
    private void Start()
    {
        GameManager.Instance.SetState(GameManager.SceneState.DeckEditor);

        foreach (PlayerProfile.PlayerCard playerCard in GameManager.Instance.GetPlayerProfile().allDeck)
        {
            if(!playerCard.unlocked) continue;

            switch (playerCard.inDeck)
            {
                case true:
                    playerDeck.AddCard(playerCard.cardData);
                    break;
                case false:
                    playerCollection.AddCard(playerCard.cardData);
                    break;
            }
        }
    }

    private void OnEnable()
    {
        InputSystem.OnCardClick += InputSystem_OnCardClick;
        InputSystem.OnEscDeck += InputSystem_OnEscDeck;
    }

    private void OnDisable()
    {
        InputSystem.OnCardClick += InputSystem_OnCardClick;
        InputSystem.OnEscDeck -= InputSystem_OnEscDeck;
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

    private void AddToDeck(BaseCard baseCard)
    {
        if (playerDeck.GetCards().Count >= MAX_DECK)
        {
            SoundManager.PlaySound(SoundManager.Sound.UiError);
            return;
        }
        
        SoundManager.PlaySound(SoundManager.Sound.UiChange);
        playerDeck.AddCard(baseCard.GetCardData());
        playerCollection.RemoveCard(baseCard);
        
        GameManager.Instance.ChangeDeck(baseCard.GetCardData(), true);
    }
    
    private void AddToCollection(BaseCard baseCard)
    {
        SoundManager.PlaySound(SoundManager.Sound.UiChange);
        
        playerCollection.AddCard(baseCard.GetCardData());
        playerDeck.RemoveCard(baseCard);
        
        GameManager.Instance.ChangeDeck(baseCard.GetCardData(), false);
    }
    
    // Use when deck is incomplete
    private void InputSystem_OnEscDeck(object sender, EventArgs e)
    {
        if (playerDeck.GetCards().Count < MAX_DECK)
        {
            DeckWarning();
        }
        else
        {
            GoBack();
        }
    }
    private void GoBack()
    {
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.UiSelect);
        GameManager.Instance.Save();
        LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
    }
    private void DeckWarning()
    {
        Debug.Log("the deck must contain " + MAX_DECK + "Cards");
    }
    
}
