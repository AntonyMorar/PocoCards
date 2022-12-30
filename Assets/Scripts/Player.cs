using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IHandeable
{
    public class OnHealthChangeEventArgs : EventArgs
    {
        public float NewHealth;
        public float Amountchange;
        public bool ApplyEffects;
    }

    public class OnAmountChangeEventArgs : EventArgs
    {
        public int Amount;
        public Player Owner;
    }

    // Public *****
    public event EventHandler OnSetUpComplete;
    public event EventHandler<OnHealthChangeEventArgs> OnHealthChange; // return the actual health
    public event EventHandler<int> OnRestoreHealth; // Return the restore amount
    public event EventHandler OnDead;
    public event EventHandler<int> OnBalanceChange;
    public event EventHandler<int> OnCoinStealed;

    // Spell events
    public event EventHandler<int> OnShieldChange;
    public event EventHandler<int> OnPoisonAdd;
    public event EventHandler<int> OnPoisonDamage;
    public event EventHandler OnPoisonRemoved;

    public static event EventHandler<OnAmountChangeEventArgs> OnSale;
    public static event EventHandler<Player> OnSaleFinished;
    public event EventHandler<int> OnDamageReduceChange;

    // Serialized *****
    [SerializeField] private Transform handAnchor;
    [SerializeField] private Transform hitPrefab;
    [Header("Card")] [SerializeField] private Card cardPrefab;

    // Private *****
    private List<CardData> _deck = new List<CardData>();
    private List<Card> _hand = new List<Card>();
    private int _baseHealth = 300;
    private int _health;
    private int _shield;
    private int _coins;
    private bool _leftSide;
    private Sprite _profilePic;

    // spells
    private int _poisonedAmount;
    private int _priceReduced;
    private int _damageReduced;

    // MonoBehaviour Callbacks *****
    private void OnEnable()
    {
        MatchManager.Instance.OnTurnChange += GameManager_OnTurnChange;
        MatchManager.Instance.OnGameOver += GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame += GameManager_OnRestartGame;
    }

    private void OnDisable()
    {
        MatchManager.Instance.OnTurnChange -= GameManager_OnTurnChange;
        MatchManager.Instance.OnGameOver -= GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame -= GameManager_OnRestartGame;
    }

    // Private Methods *****
    private void SetInitialValues()
    {
        _health = _baseHealth;
        _shield = 0;
        _coins = 0;

        _poisonedAmount = 0;
        _priceReduced = 0;
        _damageReduced = 0;
        RemoveHand();

        OnHealthChange?.Invoke(this,
            new OnHealthChangeEventArgs { NewHealth = _health, Amountchange = _baseHealth, ApplyEffects = false });
    }
    public void SetPlayer(PlayerData playerData)
    {
        _deck.Clear();
        foreach (CardData cardData in playerData.deckData.deck)
        {
            _deck.Add(cardData);
        }

        _baseHealth = playerData.baseHealth;
        _profilePic = playerData.profilePic;
        _leftSide = playerData.ally;
        SetInitialValues();

        OnSetUpComplete?.Invoke(this, EventArgs.Empty);
    }

    private void GameManager_OnTurnChange(object sender, int newTurn)
    {
        // Coins
        //AddCoins(Mathf.Clamp(newTurn, 0, 5));
        _coins = Mathf.Clamp(newTurn, 0, 6);
        OnBalanceChange?.Invoke(this,_coins);

        //Spells
        ApplyPoisonDamage();
        RemoveDamageReduce();
    }

    private void GameManager_OnGameOver(object sender, bool won)
    {
        RemoveHand();
    }
    
    private void GameManager_OnRestartGame(object sender, EventArgs e)
    {
        SetInitialValues();
    }
    
    private void RemoveHand()
    {
        _hand.Clear();
    }

    // Public Methods *****
    public void DrawCard()
    {
        if (_deck.Count <= 0 || _hand.Count > 10) return;
        
        //Audio
        if(ImOwner()) SoundManager.PlaySound(SoundManager.Sound.CardSlide);
        
        CardData randCard = _deck[Random.Range(0, _deck.Count)];
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard, ImOwner());
        AddToHand(newCard, true);
    }
    public void AddDeckToBoard()
    {
        if (_deck.Count <= 0) return;
        
        CardData randCard = _deck[Random.Range(0, _deck.Count)];
        Card newCard = Instantiate(cardPrefab, handAnchor);
        newCard.SetCard(this, randCard, true);
        newCard.AddToBoardFromDeck();
        
        //Audio
        if(ImOwner()) SoundManager.PlaySound(SoundManager.Sound.CardSlide);
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
        float cardWidth = 1.25f;
        float spacing = 0.1f;
        float totalWidth = ((_hand.Count + newCardsToAdd) * cardWidth) + ((_hand.Count + newCardsToAdd) * cardWidth ) * spacing -spacing;
        Vector3 pivotOffset = new Vector3(cardWidth/2, 0, 0);

        Vector3 startingPosition = Vector3.zero;
        startingPosition.x = -totalWidth / 2f;

        return startingPosition + Vector3.right * index * (spacing + cardWidth) + pivotOffset;
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
    public bool ImOwner() => this == MatchManager.Instance.GetMyPlayer();
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
        _health = Mathf.Clamp(_health, 0, _baseHealth);
        OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { NewHealth = _health, Amountchange = -remainDamage, ApplyEffects = true});
        if(shieldTemp != _shield) OnShieldChange?.Invoke(this, _shield);
        
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.CardAttack);
        
        float worldScreenHeight = GameManager.Instance.GetCamera().orthographicSize;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        Transform hitClone = Instantiate(hitPrefab, new Vector3(_leftSide ? -worldScreenWidth +1f : worldScreenWidth -1f,_leftSide ? -worldScreenHeight + 1 :worldScreenHeight -1f,0), Quaternion.identity);
        Destroy(hitClone.gameObject, 5f);
        
        if (_health <= 0) Died();
    }

    public void RestoreHealth(int amount)
    {
        if (_health >= _baseHealth)
        {
            //Audio
            SoundManager.PlaySound(SoundManager.Sound.CardEffectMiss);
            return;
        }
        
        _health += amount;
        
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectHeal);
        
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
        MatchManager.Instance.SetPhase(MatchManager.GamePhase.GameOver);
    }

    public int GetBaseHealth() => _baseHealth;
    public int GetHealth() => _health;
    
    // Spells
    public void PoisonEnemy(int amount)
    {
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectPoison);
        
        MatchManager.Instance.GetEnemy(this).AddPoisonDamage(amount);
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
        OnPoisonDamage?.Invoke(this, _poisonedAmount);
        
        if(_poisonedAmount <= 0) OnPoisonRemoved?.Invoke(this, EventArgs.Empty);
    }
    public bool StealCoin(int amount)
    {
        if (!MatchManager.Instance.GetEnemy(this).SpendCoins(amount))
        {
            SoundManager.PlaySound(SoundManager.Sound.CardEffectMiss);
            return false;
        }
        
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectCoin);
        
        AddCoins(amount);
        OnCoinStealed?.Invoke(this,amount);
        return true;

    }
    public void AddPriceReduce(int amount)
    {
        _priceReduced += amount;
        _priceReduced = Mathf.Clamp(_priceReduced, 0, 7);
        OnSale?.Invoke(this, new OnAmountChangeEventArgs
        {
            Amount = _priceReduced,
            Owner = this
        });
    }
    
    public void RemovePriceReduce(int amount)
    {
        if (_priceReduced <= 0) return;
        
        _priceReduced -= amount;
        OnSaleFinished?.Invoke(this,this);
    }
    public List<Card> GetHand() => _hand;
    public void AddDamageReduce(int amount)
    {
        _damageReduced += amount;
        _damageReduced = Mathf.Clamp(_damageReduced, 0, 100);
        
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectShield);
        
        OnDamageReduceChange?.Invoke(this, _damageReduced);
    }
    private void RemoveDamageReduce()
    {
        if (_damageReduced <= 0) return;

        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectFrozen);
        
        _damageReduced = 0;
        OnDamageReduceChange?.Invoke(this,_damageReduced);
    }
    public void FreezeEnemyCard(int amount)
    {
        MatchManager.Instance.GetEnemy(this).FreezeRandomCard();
    }
    private void FreezeRandomCard()
    {
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.EffectFrozen);
        
        int randomCard = Random.Range(0, _hand.Count);
        if (_hand.Count <= 0) return;
        _hand[randomCard].Freeze();
    }
    public void ChangeEnemyCardInBoard(CardData newCard)
    {
        Board.Instance.ChangeRandomCard(MatchManager.Instance.GetEnemy(this), newCard);
    }
    public Sprite GetProfilePic() => _profilePic;
}
