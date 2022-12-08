using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    // Public *****
    public event EventHandler<int> OnHealthChange;
    public event EventHandler OnDead;
    public event EventHandler<int> OnShieldChange;
    public event EventHandler<int> OnCoinsChange;
    
    // Serialized *****
    [SerializeField] private int baseHealth = 300;
    [SerializeField] private Transform handTransform;
    [Header("Card")] 
    [SerializeField] private Card cardPrefab;
    [SerializeField] private List<CardData> deck = new List<CardData>();

    // Private *****
    private int _health;
    private int _shield;
    private int _coin;
    //Hand
    private List<Card> _hand = new List<Card>();

    
    // MonoBehaviour Callbacks *****
    private void Start()
    {
        SetInitialValues();
    }
    
    // Private Methods *****

    private void SetInitialValues()
    {
        _health = baseHealth;
        OnHealthChange?.Invoke(this, _health);
    }

    private void ReorderCards()
    {
        int handSize = _hand.Count;
        float cardSize = 1f;
        int handHalf = handSize / 2;
        for (int i=0; i<handSize; i++)
        {
            float newPosX = handSize % 2 == 0 ? i - handHalf + (cardSize/2) : i - handHalf;
            Vector3 position = handTransform.position;
            _hand[i].transform.position = new Vector3(position.x + newPosX, position.y, position.z);
        }
    }
    
    // Public Methods
    public void DrawCard()
    {
        if (deck.Count <= 0)return;
        
        CardData randCard = deck[Random.Range(0, deck.Count)];
        
        Card newCard = Instantiate(cardPrefab, handTransform);
        newCard.SetCard(this, randCard);
        AddToHand(newCard);
    }

    public void AddRandomToBoard()
    {
        if (_hand.Count <= 0) return;
        int randomCard = Random.Range(0, _hand.Count);
        _hand[randomCard].AddToBoard();
    }
    
    public void AddToHand(Card card)
    {
        _hand.Add(card);
        ReorderCards();
    }
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderCards();
    }
    
    // Health
    public void TakeDamage(int damageAmount)
    {
        int shieldTemp = _shield;
        int remainDamage = damageAmount - _shield;
        if (remainDamage <= 0) remainDamage = 0;

        _shield -= damageAmount;
        _shield = Mathf.Clamp(_shield, 0, 9999);

        _health -= remainDamage;
        _health = Mathf.Clamp(_health, 0, baseHealth);
        OnHealthChange?.Invoke(this, _health);
        if(shieldTemp != _shield) OnShieldChange?.Invoke(this, _shield);
        
        if (_health <= 0) Died();
    }
    
    public void AddShield(int amount)
    {
        _shield += amount;
        OnShieldChange?.Invoke(this, amount);
    }

    public void Died()
    {
        OnDead?.Invoke(this,EventArgs.Empty);
    }
}
