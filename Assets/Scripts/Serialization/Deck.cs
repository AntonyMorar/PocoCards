using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField] private CardData[] cards = new CardData[6];

    public Deck(DeckData deckData)
    {
        for (int i = 0; i< deckData.deck.Count; i++)
        {
            cards[i] = deckData.deck[i];
        }
    }

    public CardData[] GetCards() => cards;
}
