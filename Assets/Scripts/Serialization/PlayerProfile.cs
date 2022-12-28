using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    [System.Serializable]
    public struct PlayerCard
    {
        public CardData cardData;
        public bool inDeck;
        public bool unlocked;
    }
    
    public int level;
    public int stones;
    public List<bool> levelsAvailable= new List<bool>();
    public int baseHealth = 25;

    public List<PlayerCard> playerDeck = new List<PlayerCard>();
    //public List<CardData> deckData = new List<CardData>();

    public PlayerProfile(PlayerData playerData)
    {
        level = 1;
        stones = 0;

        baseHealth = playerData.baseHealth;
        for (int i = 0; i < 3; i++)
        {
            levelsAvailable.Add(false);
            if (i <= 0) levelsAvailable[i] = true;
        }

        foreach (CardData cardData in playerData.deckData.deck)  
        {
            playerDeck.Add(new PlayerCard()
            {
                cardData = cardData,
                inDeck = true,
                unlocked = true
            });
        }
    }
}

