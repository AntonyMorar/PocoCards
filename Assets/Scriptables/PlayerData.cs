using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "ScriptableObjects/Player")]
public class PlayerData : ScriptableObject
{
    public string playerName = "Player 1";
    public bool ally;
    public Sprite profilePic;
    public int baseHealth = 25;
    public DeckData deckData;
}
