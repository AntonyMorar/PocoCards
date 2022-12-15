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
    private Camera _camera;
    private Player _owner;
    private bool _isSelectable = true;
    private bool _isActive; // Making on reveal changes
    private bool _isInBoard;
    private float _flippingTime = 0.2f;
    private bool _isFlipped;
    private bool _isFlipping;
    private int _actualCost;
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
        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    private void OnDisable()
    {
        if(_owner)_owner.OnBalanceChange -= Owner_OnBalanceChange;
        
        GameManager.Instance.OnMainStart -= GameManager_OnMainStart;
        GameManager.Instance.OnBattleStart -= GameManager_OnBattleStart;
        
        OnHideInfo?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        _camera = Camera.main;
        Player.OnPriceChange += Player_OnPriceChange;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && GameManager.Instance.GetGamePhase() != GameManager.GamePhase.GameOver)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                if (!_owner.ImOwner() || !_isSelectable || _isFrozen) return;

                if (_isInBoard) AddToHand();
                else AddToBoard();
            }
        }
        
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.I)) && GameManager.Instance.GetGamePhase() != GameManager.GamePhase.GameOver)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform && _isFlipped && _owner.ImOwner())
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
        _owner = owner;
        _cardData = cardData;
        _actualCost = _cardData.cost;
        _canAttack = CanAttack();
        
        // TODO: Remove repearted code (FlipCorrutone) i dont know how
        mainSpriteRenderer.sprite = backFaceSprite;
        artAnchor.sprite = null;
        attackPointsText.text = "";
        costText.text = "";
        
        if(flipped) Flip(false);

        _owner.OnBalanceChange += Owner_OnBalanceChange;
        CheckBalanceAvailability(_owner.GetCoins());
    }

    public void Flip(bool animated)
    {
        LeanTween.cancel(gameObject);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (animated) LeanTween.rotateY(gameObject, 90, _flippingTime/2).setLoopPingPong(1);
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
            attackPointsText.text = _cardData.attackPoints.ToString();
            costText.text = _actualCost.ToString();
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

    public bool ImOwner() => _owner.ImOwner();
    public int GetActualCost() => _actualCost;
    public void AddToBoardFromDeck()
    {
        _isInBoard = true;
        Board.Instance.AddToHand(this, true);
    }

    public void Freeze()
    {
        frostImage.SetActive(true);
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

    // Add to board from hand
    private void AddToHand()
    {
        _isInBoard = false;
        _owner.AddToHand(this);
        Board.Instance.RemoveFromHand(this);
        
        _owner.RemovePriceReduce();
        
        // Change player currency
        _owner.AddCoins(_actualCost);
    }
    public void AddToBoard()
    {
        if (_owner.GetCoins() < _actualCost - _owner.GetPriceReduce()) return;
        
        _isInBoard = true;
        _owner.RemoveFromHand(this);
        Board.Instance.AddToHand(this);
        
        // Change player currency
        _owner.SpendCoins(_actualCost - _owner.GetPriceReduce());
        
        // Apply Immediate effects
        // Next card cost less
        if (_cardData.reduceNextCardCost > 0)
        {
            _owner.AddPriceReduce(_cardData.reduceNextCardCost);
        }
    }

    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
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

    private void MakeAttack(float time)
    {
        LeanTween.moveY(gameObject,  GameManager.Instance.GetMyPlayer() == _owner ? transform.position.y + 0.3f: transform.position.y -0.3f, time/2).setEase(LeanTweenType.easeInBack).setLoopPingPong(1).setOnComplete(
            () =>
            {
                GameManager.Instance.TakeDamage(GameManager.Instance.GetEnemy(_owner), _cardData.attackPoints);
            });
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
        
        // Backstab

    }

    private bool CanAttack()
    {
        if (_cardData.enemyHasMoreHealth && _cardData.attackPoints > 0)
        {
            if (GameManager.Instance.GetEnemy(_owner).GetHealth() > _owner.GetHealth())
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
        SetLock(newBalance < _actualCost);
    }
    private void SetLock(bool setLock)
    {
        if (!_owner.ImOwner()) return;
        _isSelectable = !setLock;

        if (_isInBoard) return;
        if(!_isFrozen) blockedCard.SetActive(setLock);
    }
    
    // Immediate effects
    private void Player_OnPriceChange(object sender, Player.OnPriceChangeEventArgs priceArgs)
    {
        if (priceArgs.Owner != _owner) return;
        
        _actualCost = _cardData.cost - priceArgs.AmountChange;
        costText.text = _actualCost.ToString();
    }
}
