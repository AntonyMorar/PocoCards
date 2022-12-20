using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class DeckData : ScriptableObject
{
    public List<CardData> deck;
}
