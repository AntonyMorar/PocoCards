using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "ScriptableObjects/Deck")]
[System.Serializable]
public class DeckData : ScriptableObject
{
    public List<CardData> deck;
}
