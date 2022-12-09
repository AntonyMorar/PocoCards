using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IHandeable
{
    // Public *****
    public event EventHandler<int> OnHealthChange;
    public event EventHandler OnDead;
    public event EventHandler<int> OnShieldChange;
    public event EventHandler<int> OnBalanceChange;
    
    // Serialized *****
    [SerializeField] private int baseHealth = 300;
    [SerializeField] private Transform handAnchor;
    [Header("Card")] 
    [SerializeField] private Card cardPrefab;
    [SerializeField] private List<CardData> deck = new List<CardData>();

    // Private *****
    private int _health;
    private int _shield;
    private int _coins;
    //Hand
    private List<Card> _hand = new List<Card>();

    
    // MonoBehaviour Callbacks *****
    private void OnEnable()
    {
        GameManager.Instance.OnTurnChange += GameManager_OnTurnChange;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.OnTurnChange -= GameManager_OnTurnChange;
    }

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

    private void GameManager_OnTurnChange(object sender, int newTurn)
    {
        AddCoins(newTurn);
    }

    // Public Methods *****
    public void DrawCard()
    {
        if (deck.Count <= 0 || _hand.Count > 30) return;
        
        CardData randCard = deck[Random.Range(0, deck.Count)];
        
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard);
        AddToHand(newCard);
    }
    public void AddDeckToBoard()
    {
        if (deck.Count <= 0) return;
        
        CardData randCard = deck[Random.Range(0, deck.Count)];
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard);
        newCard.AddToBoard();
    }
    
    public void AddToHand(Card card)
    {
        _hand.Add(card);
        card.transform.SetParent(handAnchor, false);
        
        ReorderCards();
    }
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderCards();
    }
    public void ReorderCards()
    {
        int handSize = _hand.Count;
        if (handSize <= 0) return;
        
        float cardSize = 1f;
        int handHalf = handSize / 2;
        for (int i=0; i<handSize; i++)
        {
            float newPosX = handSize % 2 == 0 ? i - handHalf + (cardSize/2) : i - handHalf;
            _hand[i].transform.localPosition = new Vector3(newPosX, 0f, 0);
        }
    }
    public int GetCoins() => _coins;
    public void AddCoins(int amount)
    {
        _coins += amount;
        _coins = Mathf.Clamp(_coins, 0, 999);
        OnBalanceChange?.Invoke(this,_coins);
    }
    public bool SpendCoins(int amount)
    {
        if (_coins <= 0) return false;
        
        _coins -= amount;
        _coins = Mathf.Clamp(_coins, 0, 999);
        OnBalanceChange?.Invoke(this,_coins);
        return true;
    }
    public bool ImOwner() => this == GameManager.Instance.GetMyPlayer();
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
    
    // Spells
    public void PoisonEnemy(int amount)
    {
        GameManager.Instance.GetEnemy(this).AddPoison(amount);
    }
    public void AddPoison(int amount)
    {
        Debug.Log("Poison not working");
    }

    public bool StealCoin(int amount)
    {
        if (GameManager.Instance.GetEnemy(this).SpendCoins(amount))
        {
            AddCoins(amount);
            return true;
        }
        
        return false;
    }
}
