using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite cardIcon;
    public int attackPoints;
    public int cost = 1;

    [Header("Special effects")] 
    public bool hasSpecialEffect;
    [Tooltip("Add random card from deck to your hand")]
    public int addRandomCardHand;
    [Tooltip("Add random card from deck to the board")]
    public int addRandomCardBoard;
    public int addShield;
    [Tooltip("Poisoning to your opponent, affects next turn")]
    public int addEnemyPoison;
    public int stealCoin;
    [Tooltip("Reduce card cost next turn")]
    public int reduceCardCost;
    public int reduceAllDamage;


    [Header("Effects conditions")] 
    [Tooltip("Condition affect if there this number of enemy cards in the board")]
    public int enemyCardsInBoard;
    [Tooltip("Condition affect if there this number of your cards in the board")]
    public int allyCardsInBoard;
}
