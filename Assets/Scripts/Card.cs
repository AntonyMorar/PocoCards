using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    // Serialized **********
    [SerializeField] private LayerMask layer;
    [Header("References")]
    [SerializeField] private SpriteRenderer artAnchor;
    [SerializeField] private TMP_Text attackPointsText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private GameObject blockedCard;
    // Private **********
    private CardData _cardData;
    private Camera _camera;
    private Player _owner;
    private bool _isSelectable = true;
    private bool _isActive; // Making on reveal changes
    private bool _isInBoard;

    // Actions & State
    private State _state;
    private float _stateTimer;

    private Action _onRevealComplete;
    private enum State
    {
        Flipping,
        SpecialEffect,
        Attacking,
        CoolOff
    }

    // MonoBehaviour Callbacks **********
    private void OnEnable()
    {
        if (_owner) _owner.OnBalanceChange += Owner_OnBalanceChange;
    }

    private void OnDisable()
    {
        if(_owner) _owner.OnBalanceChange -= Owner_OnBalanceChange;
    }

    private void Start()
    {
        _camera = Camera.main;

        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                if (!_owner.ImOwner() || !_isSelectable) return;

                if (_isInBoard) AddToHand();
                else AddToBoard();
            }
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
                    if(_cardData.hasSpecialEffect) StartCoroutine(SpecialEffectAnimation(specialEffectTime));
                    // Timer for special effect
                    _stateTimer = specialEffectTime;
                    break;
                case State.SpecialEffect:
                    _state = State.Attacking;
                    float attackTime = _cardData.attackPoints > 0 ? 0.5f : 0;
                    // Start attack
                    if(_cardData.attackPoints > 0) MakeAttack(attackTime);
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
    public void SetCard(Player owner, CardData cardData)
    {
        _owner = owner;
        _cardData = cardData;

        artAnchor.sprite = _cardData.cardIcon;
        attackPointsText.text = cardData.attackPoints.ToString();
        costText.text = cardData.cost.ToString();
        
        _owner.OnBalanceChange += Owner_OnBalanceChange;
        CheckBalanceAvailability(_owner.GetCoins());
    }
    
    public void TakeAction(Action onRevealComplete)
    {
        ActionStart(onRevealComplete);

        _state = State.Flipping;
        _stateTimer = 0.75f;
    }
    
    public void Remove()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setDestroyOnComplete(true);
    }

    public bool ImOwner() => _owner.ImOwner();

    public void AddToBoardFromDeck()
    {
        _isInBoard = true;
        Board.Instance.AddToHand(this);
    }
    
    // Private Methods **********
    private void AddToHand()
    {
        _isInBoard = false;
        _owner.AddToHand(this);
        Board.Instance.RemoveFromHand(this);
        
        // Change player currency
        _owner.AddCoins(_cardData.cost);
    }
    
    // Add to board from hand
    private void AddToBoard()
    {
        if (_owner.GetCoins() < _cardData.cost - _owner.GetPriceReduce()) return;
        
        _isInBoard = true;
        _owner.RemoveFromHand(this);
        Board.Instance.AddToHand(this);
        
        // Change player currency
        _owner.SpendCoins(_cardData.cost - _owner.GetPriceReduce());
    }

    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        SetLock(false);
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

    private IEnumerator SpecialEffectAnimation(float time)
    {
        LeanTween.scale(gameObject, new Vector3(1.33f, 1.33f, 1.33f), time / 2).setEase(LeanTweenType.easeOutExpo)
            .setLoopPingPong(1);

        yield return new WaitForSeconds((time / 2) + time * 0.2f);
        SpecialEffect();
    }
    private void SpecialEffect()
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
        
        // Next card cost less
        if (_cardData.reduceNextCardCost > 0)
        {
            _owner.AddPriceReduce(_cardData.reduceNextCardCost);
        }
        
        // Next turn recive less damage
        if (_cardData.reduceTurnDamage > 0)
        {
            _owner.AddDamageReduce(_cardData.reduceTurnDamage);
        }
        
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
        SetLock(newBalance < _cardData.cost);
    }
    private void SetLock(bool setLock)
    {
        if (!_owner.ImOwner()) return;
        _isSelectable = !setLock;

        if (_isInBoard) return;
        blockedCard.SetActive(setLock);
    }
}
