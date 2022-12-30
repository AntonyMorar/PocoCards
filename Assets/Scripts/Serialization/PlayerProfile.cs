using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    [System.Serializable]
    public class PlayerCard
    {
        public CardData cardData;
        public bool inDeck;
        public bool unlocked;

        public void Unlock()
        {
            unlocked = true;
        }
    }
    
    public int level;
    public int stones;
    public List<bool> levelsAvailable= new List<bool>();
    public int baseHealth = 25;

    public List<PlayerCard> allDeck = new List<PlayerCard>();

    public PlayerProfile(PlayerData playerData, DeckData availableCards)
    {
        level = 1;
        stones = 0;

        baseHealth = playerData.baseHealth;
        for (int i = 0; i < 3; i++)
        {
            levelsAvailable.Add(false);
            if (i <= 0) levelsAvailable[i] = true;
        }

        foreach (CardData cardData in availableCards.deck)  
        {
            allDeck.Add(new PlayerCard()
            {
                cardData = cardData,
                inDeck = playerData.deckData.deck.Contains(cardData),
                unlocked = playerData.deckData.deck.Contains(cardData)
            });
        }
    }

    public CardData GetLockedRandomCard()
    {
        List<CardData> lockedCards = new List<CardData>();
        foreach (PlayerCard playerCard in allDeck)
        {
            if (!playerCard.unlocked)
            {
                lockedCards.Add(playerCard.cardData);
            }
        }

        if (lockedCards.Count <= 0) return null;
        int randomNumber = Random.Range(0, lockedCards.Count);
        
        return lockedCards[randomNumber];
    }
}

