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
    private bool _canDoSpecialEffect;
    private bool _canAttack;

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
        if (Input.GetMouseButtonDown(0))
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
        switch (_state)
        {
            case State.Flipping:
                break;
            case State.SpecialEffect:
                if (_canDoSpecialEffect)
                {
                    SpecialEffect();
                    _canDoSpecialEffect = false;
                }
                break;
            case State.Attacking:
                if (_canAttack)
                {
                    MakeAttack();
                    _canAttack = false;
                }
                break;
            case State.CoolOff:
                break;
        }
        
        if (_stateTimer <= 0)
        {
            switch (_state)
            {
                case State.Flipping:
                    _state = State.SpecialEffect;
                    _stateTimer = _cardData.hasSpecialEffect ? 2f : 0f;
                    break;
                case State.SpecialEffect:
                    _state = State.Attacking;
                    _stateTimer = 0.5f;
                    break;
                case State.Attacking:
                    _state = State.CoolOff;
                    _stateTimer = 0.5f;
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
        _stateTimer = 1f;

        _canDoSpecialEffect = true;
        _canAttack = true;
    }
    
    public void AddToBoard()
    {
        if (_owner.GetCoins() < _cardData.cost) return;
        
        transform.position += _owner.ImOwner() ? Vector3.up * 2.5f : Vector3.up * -2.5f;
        _isInBoard = true;
        
        _owner.RemoveFromHand(this);
        Board.Instance.AddToBoardHand(this, _owner);
        // Change player currency
        _owner.SpendCoins(_cardData.cost);
    }

    public void Remove()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setDestroyOnComplete(true);
    }
    
    // Private Methods **********
    private void AddToHand()
    {
        transform.position += Vector3.down * 2.5f;
        _isInBoard = false;
        
        _owner.AddToHand(this);
        Board.Instance.RemoveFromBoardHand(this);
        // Change player currency
        _owner.AddCoins(_cardData.cost);
    }

    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        _isSelectable = true;
    }
    
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        _isSelectable = false;
    }

    private void MakeAttack()
    {
        LeanTween.moveY(gameObject,  GameManager.Instance.GetMyPlayer() == _owner ? transform.position.y + 0.3f: transform.position.y -0.3f, 0.25f).setEase(LeanTweenType.easeInBack).setLoopPingPong(1).setOnComplete(
            () =>
            {
                GameManager.Instance.TakeDamage(GameManager.Instance.GetEnemy(_owner), _cardData.attackPoints);
            });
    }

    private void SpecialEffect()
    {
        // Add Random card to hand
        if (_cardData.addRandomCardHand > 0)
        {
            Debug.Log("Add Random card to hand");
            _owner.DrawCard();
        }
        
        // Add random card from deck to the board
        if (_cardData.addRandomCardBoard > 0)
        {
            Debug.Log("NOT WORKING");
        }
        
        // Add Shield
        if (_cardData.addShield > 0)
        {
            Debug.Log("Adding shield");
            _owner.AddShield(_cardData.addShield);
        }

        if (_cardData.addEnemyPoison > 0)
        {
            Debug.Log("addEnemyPoison");
            _owner.PoisonEnemy(_cardData.addEnemyPoison);
        }
        
    }

    private void ActionStart(Action onRevealComplete)
    {
        _isActive = true;
        _onRevealComplete = onRevealComplete;
    }
    private void ActionComplete()
    {
        Debug.Log((_owner.ImOwner() ? "Player " : "Enemy ") + "complete: " + _cardData.cardName);
        _isActive = false;
        _onRevealComplete();
    }

    private void Owner_OnBalanceChange(object sender, int newBalance)
    {
        CheckBalanceAvailability(newBalance);
    }
    private void CheckBalanceAvailability(int newBalance)
    {
        if (!_owner.ImOwner() || _isInBoard) return;
        
        if (newBalance < _cardData.cost)
        {
            blockedCard.SetActive(true);
        }
        else
        {
            blockedCard.SetActive(false);
        }
    }
}
