using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public int level;
    public int stones;
    public int maxHealth;
    public int levelCompleted;
    public List<CardData> deck;
    public List<CardData> collection;

    public PlayerProfile(DeckData deckData, DeckData collectionData)
    {
        playerName = "Player 1";
        level = 1;
        stones = 0;
        maxHealth = 25;
        levelCompleted = 1;
        deck = deckData.deck;
        deck = new List<CardData>();
        foreach (CardData cardData in deckData.deck)
        {
            deck.Add(cardData);
        }
        collection = new List<CardData>();
        foreach (CardData cardData in collectionData.deck)
        {
            if(!deckData.deck.Contains(cardData))
                collection.Add(cardData);
        }
    }
}
