using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IHandeable
{
    public class OnHealthChangeEventArgs: EventArgs
    {
        public float NewHealth;
        public float Amountchange;
        public bool ApplyEffects;
    }
    
    public class OnPriceChangeEventArgs: EventArgs
    {
        public int AmountChange;
        public Player Owner;
    }
    // Public *****
    public event EventHandler<OnHealthChangeEventArgs> OnHealthChange; // return the actual health
    public event EventHandler<int> OnRestoreHealth; // Return the restore amount
    public event EventHandler OnDead;
    public event EventHandler<int> OnBalanceChange;

    public event EventHandler<int> OnCoinStealed;
    // Spell events
    public event EventHandler<int> OnShieldChange;
    public event EventHandler<int> OnPoisonAdd;
    public event EventHandler OnPoisonRemoved;
    public static event EventHandler<OnPriceChangeEventArgs> OnPriceChange;
    public event EventHandler<int> OnDamageReduceChange;
    
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
    // spells
    private int _poisonedAmount;
    private int _priceReduced;
    private int _damageReduced;
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
        OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { NewHealth = _health, Amountchange = baseHealth, ApplyEffects = false});
    }

    private void GameManager_OnTurnChange(object sender, int newTurn)
    {
        // Coins
        AddCoins(Mathf.Clamp(newTurn, 0, 5));
        
        //Spells
        ApplyPoisonDamage();
        RemoveDamageReduce();
    }

    // Public Methods *****
    public void DrawCard()
    {
        if (deck.Count <= 0 || _hand.Count > 10) return;
        
        CardData randCard = deck[Random.Range(0, deck.Count)];
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard, ImOwner());
        AddToHand(newCard, true);
    }
    public void AddDeckToBoard()
    {
        if (deck.Count <= 0) return;
        
        CardData randCard = deck[Random.Range(0, deck.Count)];
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard, true);
        newCard.AddToBoardFromDeck();
    }
    
    public void AddToHand(Card card, bool isNew = false)
    {
        ReorderActualCards(1);
        card.transform.SetParent(handAnchor);

        if (isNew) card.transform.position = new Vector3(0, ImOwner() ? -6 : 6, 0);

        LeanTween.moveLocal(card.gameObject, GetCardPositionInHand(_hand.Count,1), 0.15f);
        _hand.Add(card);
    }
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderActualCards(0);
    }
    public void ReorderActualCards(int newCardsToAdd)
    {
        for (int i=0; i<_hand.Count; i++)
        {
            LeanTween.moveLocal(_hand[i].gameObject, GetCardPositionInHand(i, newCardsToAdd), 0.15f);
        }
    }

    private Vector3 GetCardPositionInHand(int index, int newCardsToAdd)
    {
        int handSize = _hand.Count + newCardsToAdd;
        if (handSize <= 0) return Vector3.zero;
        
        float cardSize = 1.1f;
        int handHalf = handSize / 2;
        
        float newPosX = handSize % 2 == 0 ? index*cardSize - handHalf + (cardSize/2) : index*cardSize - handHalf;
        return new Vector3(newPosX, 0f, 0);
    }
    public int GetCoins() => _coins;
    public void AddCoins(int amount)
    {
        _coins += amount;
        _coins = Mathf.Clamp(_coins, 0, 50);
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
    public void TakeDamage(int damageAmount, bool ignoreProtection = false)
    {
        int shieldTemp = _shield;
        int remainDamage = damageAmount - (ignoreProtection ? 0 :_damageReduced);
        remainDamage -= _shield;
        if (remainDamage <= 0) remainDamage = 0;

        _shield -= damageAmount;
        _shield = Mathf.Clamp(_shield, 0, 9999);

        _health -= remainDamage;
        _health = Mathf.Clamp(_health, 0, baseHealth);
        OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { NewHealth = _health, Amountchange = -remainDamage, ApplyEffects = true});
        if(shieldTemp != _shield) OnShieldChange?.Invoke(this, _shield);
        
        if (_health <= 0) Died();
    }

    public void RestoreHealth(int amount)
    {
        if (_health >= baseHealth) return;
        
        _health += amount;
        OnRestoreHealth?.Invoke(this,amount);
        OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { NewHealth = _health, Amountchange = amount, ApplyEffects = true});
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

    public int GetBaseHealth() => baseHealth;
    
    // Spells
    public void PoisonEnemy(int amount)
    {
        GameManager.Instance.GetEnemy(this).AddPoisonDamage(amount);
    }
    private void AddPoisonDamage(int amount)
    {
        _poisonedAmount += amount;
        OnPoisonAdd?.Invoke(this, _poisonedAmount);
    }
    private void ApplyPoisonDamage()
    {
        if (_poisonedAmount <= 0) return;
        
        TakeDamage(1, true);
        _poisonedAmount -= 1;
        _poisonedAmount = Mathf.Clamp(_poisonedAmount, 0, 999);
        
        if(_poisonedAmount <= 0) OnPoisonRemoved?.Invoke(this, EventArgs.Empty);
    }
    public bool StealCoin(int amount)
    {
        if (!GameManager.Instance.GetEnemy(this).SpendCoins(amount)) return false;
        AddCoins(amount);
        OnCoinStealed?.Invoke(this,amount);
        return true;

    }
    public void AddPriceReduce(int amount)
    {
        _priceReduced += amount;
        _priceReduced = Mathf.Clamp(_priceReduced, 0, 7);
        OnPriceChange?.Invoke(this, new OnPriceChangeEventArgs
        {
            AmountChange = _priceReduced,
            Owner = this
        });
    }
    public void RemovePriceReduce()
    {
        if (_priceReduced <= 0) return;
        
        _priceReduced = 0;
        OnPriceChange?.Invoke(this, new OnPriceChangeEventArgs
        {
            AmountChange = _priceReduced,
            Owner = this
        });
    }
    public int GetPriceReduce() => _priceReduced;
    public List<Card> GetHand() => _hand;
    public void AddDamageReduce(int amount)
    {
        _damageReduced += amount;
        _damageReduced = Mathf.Clamp(_damageReduced, 0, 100);
        OnDamageReduceChange?.Invoke(this, _damageReduced);
    }
    private void RemoveDamageReduce()
    {
        if (_damageReduced <= 0) return;

        _damageReduced = 0;
        OnDamageReduceChange?.Invoke(this,_damageReduced);
    }


}
