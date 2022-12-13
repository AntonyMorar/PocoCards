using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    [Multiline]
    public string description;
    public Sprite cardIcon;
    public int attackPoints;
    public int cost = 1;
    
    [Header("Battle effects *****")] 
    public bool hasSpecialEffect = true;
    
    [Header("Draw")]
    [Tooltip("Draws a card from your deck")]
    [Range(0,10)]
    public int drawToHand;
    [Tooltip("Draws a card int the board from your deck")]
    [Range(0,10)]
    public int drawToBoard;
    
    [Header("Health")]
    public int restoreHealth;
    public int addShield;
    [Tooltip("Poisoning to your opponent, affects next turn")]
    public int addPoison;
    
    [Header("Balance")]
    [Range(0,20)]
    public int stealCoin;

    [Header("Damage")]
    public int reduceTurnDamage;
    
    [Header("Inmediate effects *****")] 
    [Tooltip("Reduce the cost of the next card, spell end at the end of the turn")]
    [Range(0,5)]
    public int reduceNextCardCost;

    [Header("Conditions *****")] 
    [Tooltip("Condition affect if there this number of enemy cards in the board")]
    public int enemyCardsInBoard;
    [Tooltip("Condition affect if there this number of your cards in the board")]
    public int allyCardsInBoard;
}
