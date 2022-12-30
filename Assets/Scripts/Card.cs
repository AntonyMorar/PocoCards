using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    // Public
    public static event EventHandler<string> OnShowInfo;
    public static event EventHandler OnHideInfo;
    public static event EventHandler OnMakeHit;
    // Serialized **********
    [SerializeField] private LayerMask layer;
    [SerializeField] private Sprite faceSprite;
    [SerializeField] private Sprite backFaceSprite;
    [Header("References")]
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    [SerializeField] private SpriteRenderer artAnchor;
    [SerializeField] private TMP_Text attackPointsText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private GameObject blockedCard;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private GameObject frostImage;
    // Private **********
    private CardData _cardData;
    private Player _owner;
    private bool _isSelectable = true;
    private bool _isActive; // Making on reveal changes
    private bool _isInBoard;
    private float _flippingTime = 0.2f;
    private bool _isFlipped;
    private bool _isFlipping;
    private int _priceReduction;
    private bool _isFrozen;
    private int _frozenTurns;
    private bool _canAttack;

    // Actions & State
    private State _state;
    private float _stateTimer;

    private Action _onRevealComplete;
    private enum State
    {
        Idle,
        Flipping,
        SpecialEffect,
        Attacking,
        CoolOff
    }

    // MonoBehaviour Callbacks **********
    private void OnEnable()
    {
        if (_owner) _owner.OnBalanceChange += Owner_OnBalanceChange;
        MatchManager.Instance.OnMainStart += GameManager_OnMainStart;
        MatchManager.Instance.OnBattleStart += GameManager_OnBattleStart;
        MatchManager.Instance.OnGameOver += GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame += GameManager_OnRestartValues;
    }

    private void OnDisable()
    {
        if(_owner)_owner.OnBalanceChange -= Owner_OnBalanceChange;
        
        MatchManager.Instance.OnMainStart -= GameManager_OnMainStart;
        MatchManager.Instance.OnBattleStart -= GameManager_OnBattleStart;
        MatchManager.Instance.OnGameOver -= GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame -= GameManager_OnRestartValues;
        
        OnHideInfo?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        Player.OnSale += Player_OnSale;
        Player.OnSaleFinished += Player_OnSaleFinished;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && MatchManager.Instance.GetGamePhase() != MatchManager.GamePhase.GameOver)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(GameManager.Instance.GetCamera().ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                if ((_owner && !_owner.ImOwner()) || !_isSelectable || _isFrozen) return;

                if (_isInBoard) AddToHand();
                else AddToBoard();
            }
        }
        
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.I)) && MatchManager.Instance.GetGamePhase() != MatchManager.GamePhase.GameOver)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(GameManager.Instance.GetCamera().ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform && _isFlipped && _owner && _owner.ImOwner())
            {
                OnShowInfo?.Invoke(this, _cardData.description);
            }
        }

        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.I))
        {
            OnHideInfo?.Invoke(this, EventArgs.Empty);
        }
        
        UpdateState();
    }

    private void UpdateState()
    {
        if (!_isActive) return;
        
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0)
        {
            switch (_state)
            {
                case State.Flipping:
                    _state = State.SpecialEffect;
                    float specialEffectTime = _cardData.hasSpecialEffect ? 1.5f : 0;
                    //Start special effect animation
                    if(_cardData.hasSpecialEffect) StartCoroutine(BattleEffectAnimation(specialEffectTime));
                    // Timer for special effect
                    _stateTimer = specialEffectTime;
                    break;
                case State.SpecialEffect:
                    _state = State.Attacking;
                    float attackTime = _canAttack ? 0.5f : 0;
                    // Start attack
                    if(_canAttack) MakeAttack(attackTime);
                    _stateTimer = attackTime;
                    break;
                case State.Attacking:
                    _state = State.CoolOff;
                    float coolOfTime = 0.5f;
                    _stateTimer = coolOfTime;
                    break;
                case State.CoolOff:
                    ActionComplete();
                    break;
            }
        }
    }

    // Public Methods **********
    public void SetCard(Player owner, CardData cardData, bool flipped)
    {
        if(owner) _owner = owner;
        _cardData = cardData;
        _canAttack = CanAttack();
        
        // TODO: Remove repeated code (FlipCorrutine) i dont know how
        mainSpriteRenderer.sprite = backFaceSprite;
        artAnchor.sprite = null;
        attackPointsText.text = "";
        costText.text = "";
        
        if(flipped) Flip(false);

        if(owner) _owner.OnBalanceChange += Owner_OnBalanceChange;
        CheckBalanceAvailability(_owner.GetCoins());
    }

    public void Flip(bool animated)
    {
        LeanTween.cancel(gameObject);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (animated)
        {
            LeanTween.rotateY(gameObject, 90, _flippingTime/2).setLoopPingPong(1);
            //Audio
            SoundManager.PlaySound(SoundManager.Sound.CardFlip);
        }

        StartCoroutine(FlipCorrutine(animated));
    }

    private IEnumerator FlipCorrutine(bool animated)
    {
        yield return new WaitForSeconds(animated ? _flippingTime/2 : 0);
        
        if (_isFlipped)
        {
            mainSpriteRenderer.sprite = backFaceSprite;
            artAnchor.sprite = null;
            coinImage.SetActive(false);
            attackPointsText.text = "";
            costText.text = "";
        }
        else
        {
            mainSpriteRenderer.sprite = faceSprite;
            artAnchor.sprite = _cardData.cardIcon;
            coinImage.SetActive(true);
            attackPointsText.text = (_cardData.attackPoints).ToString();
            costText.text = (_cardData.cost - _priceReduction).ToString();
        }
        
        _isFlipped = !_isFlipped;
    }
    
    public void TakeAction(Action onRevealComplete)
    {
        ActionStart(onRevealComplete);

        _state = State.Flipping;
        _stateTimer = _flippingTime + 0.5f;
        if(!_isFlipped) Flip(true);
    }
    
    public void Remove()
    {
        LeanTween.moveY(gameObject, transform.position.y + 0.3f, 0.4f);
        LeanTween.scale(gameObject, Vector3.zero, 0.25f).setDestroyOnComplete(true);
    }

    public bool ImOwner() => _owner && _owner.ImOwner();

    public Player GetOwner() => _owner;
    public int GetActualCost() => _cardData.cost - _priceReduction;
    public void AddToBoardFromDeck()
    {
        _isInBoard = true;
        Board.Instance.AddToHand(this, true);
    }

    public void Freeze()
    {
        frostImage.SetActive(true);
        LeanTween.alpha(frostImage, 0, 0);
        LeanTween.alpha(frostImage, 1, 0.5f).setEase(LeanTweenType.easeInCubic);
            
        _frozenTurns = 1;
        _isFrozen = true;
    }

    private void Defrost()
    {
        frostImage.SetActive(false);
        _frozenTurns = 0;
        _isFrozen = false;
    }

    public Boolean IsNotSelectableOrFrozen() => !_isSelectable || _isFrozen;
    
    public void ChangeCard( Player owner, CardData newCard)
    {
        SetCard(owner, newCard, _isFlipped);
    }

    // Add to board from hand
    private void AddToHand()
    {
        _isInBoard = false;
        _owner.AddToHand(this);
        Board.Instance.RemoveFromHand(this);

        if (_cardData.reduceNextCardCost > 0)
        {
            _owner.RemovePriceReduce(_cardData.reduceNextCardCost);
        }
        if (_priceReduction > 0)
        {
            _owner.RemovePriceReduce(_priceReduction);
        }
        
        //Audio
        if(ImOwner()) SoundManager.PlaySound(SoundManager.Sound.CardSlide);

        // Change player currency
        _owner.AddCoins(_cardData.cost - _priceReduction);
    }
    public void AddToBoard()
    {
        if (_owner.GetCoins() < _cardData.cost - _priceReduction) return;
        
        _isInBoard = true;
        _owner.RemoveFromHand(this);
        Board.Instance.AddToHand(this);
        
        // Change player currency
        _owner.SpendCoins(_cardData.cost - _priceReduction);
        
        //Audio
        if(ImOwner()) SoundManager.PlaySound(SoundManager.Sound.CardSlide);
        
        // Apply Immediate effects
        // Next card cost less
        if (_cardData.reduceNextCardCost > 0)
        {
            _owner.AddPriceReduce(_cardData.reduceNextCardCost);
        }
    }

    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        _owner.RemovePriceReduce(_priceReduction);
        CheckBalanceAvailability(_owner.GetCoins());

        if (!_isFrozen) return;
        if (_frozenTurns <= 0)
        {
            Defrost();
        }
        _frozenTurns--;
    }
    
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        SetLock(true);
    }
    private void GameManager_OnGameOver(object sender, bool won)
    {
        Remove();
    }
    private void GameManager_OnRestartValues(object sender, EventArgs e)
    {
        Remove();
    }
    
    private void MakeAttack(float time)
    {
        LeanTween.moveY(gameObject,  MatchManager.Instance.GetMyPlayer() == _owner ? transform.position.y + 0.3f: transform.position.y -0.3f, time/2).setEase(LeanTweenType.easeInBack).setLoopPingPong(1).setOnComplete(
            () =>
            {
                MatchManager.Instance.TakeDamage(MatchManager.Instance.GetEnemy(_owner), _cardData.attackPoints);
            });

        OnMakeHit?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator BattleEffectAnimation(float time)
    {
        LeanTween.scale(gameObject, new Vector3(1.25f, 1.25f, 1.25f), ((time*0.8f) / 2)).setEase(LeanTweenType.easeOutExpo).setLoopPingPong(1);
        yield return new WaitForSeconds((time / 2) + (time * 0.1f));

        BattleEffect();
    }
    private void BattleEffect()
    {
        // Add Random card to hand
        if (_cardData.drawToHand > 0)
        {
            for (int i = 0; i < _cardData.drawToHand; i++)
            {
                _owner.DrawCard();
            }
        }
        
        // Add random card from deck to the board
        if (_cardData.drawToBoard > 0)
        {
            for (int i = 0; i < _cardData.drawToBoard; i++)
            {
                _owner.AddDeckToBoard();
            }
        }
        
        // Restore Health
        if (_cardData.restoreHealth> 0)
        {
            _owner.RestoreHealth(_cardData.restoreHealth);
        }
        
        // Add Shield
        if (_cardData.addShield > 0)
        {
            _owner.AddShield(_cardData.addShield);
        }
        
        // Steal coin from opponent
        if (_cardData.stealCoin > 0)
        {
            _owner.StealCoin(_cardData.stealCoin);
        }
        
        // Poison an enemy
        if (_cardData.addPoison > 0)
        {
            _owner.PoisonEnemy(_cardData.addPoison);
        }
        
        // Next turn recive less damage
        if (_cardData.reduceTurnDamage > 0)
        {
            _owner.AddDamageReduce(_cardData.reduceTurnDamage);
        }
        
        // Freeze 
        if (_cardData.freeze > 0)
        {
            _owner.FreezeEnemyCard(_cardData.freeze);
        }

        // Change card
        if (_cardData.cardTransform != null)
        {
            _owner.ChangeEnemyCardInBoard(_cardData.cardTransform);
        }

    }

    private bool CanAttack()
    {
        if (_cardData.enemyHasMoreHealth && _cardData.attackPoints > 0)
        {
            if (MatchManager.Instance.GetEnemy(_owner).GetHealth() > _owner.GetHealth())
            {
                return true;
            }
        }
        else
        {
            return _cardData.attackPoints > 0;
        }
        
        return false;
    }

    private void ActionStart(Action onRevealComplete)
    {
        _isActive = true;
        _onRevealComplete = onRevealComplete;
    }
    private void ActionComplete()
    {
        _isActive = false;
        _onRevealComplete();
    }

    private void Owner_OnBalanceChange(object sender, int newBalance)
    {
        CheckBalanceAvailability(newBalance);
    }
    private void CheckBalanceAvailability(int newBalance)
    {
        if (_isInBoard) return;
        SetLock(_cardData.cost - _priceReduction > newBalance);
    }
    private void SetLock(bool setLock)
    {
        if (!_owner.ImOwner()) return;
        _isSelectable = !setLock;

        if (_isInBoard) return;
        if(!_isFrozen) blockedCard.SetActive(setLock);
    }
    
    // Immediate effects
    private void Player_OnSale(object sender, Player.OnAmountChangeEventArgs priceArgs)
    {
        if (priceArgs.Owner != _owner) return;

        _priceReduction = priceArgs.Amount;
        costText.text = (_cardData.cost - _priceReduction).ToString();
        CheckBalanceAvailability(_owner.GetCoins());
    }
    
    private void Player_OnSaleFinished(object sender, Player owner)
    {
        if (owner != _owner || _isInBoard) return;

        _priceReduction = 0;
        costText.text = (_cardData.cost - _priceReduction).ToString();
    }
    
    
}
