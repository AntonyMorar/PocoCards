using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Serialized **********
    [SerializeField] private LayerMask layer;
    [Header("References")]
    [SerializeField] private SpriteRenderer artAnchor;
    // Private **********
    private CardData _cardData;
    private Camera _camera;
    private bool _isSelectable = true;
    private Player _owner;
    private bool _isActive; // Making on reveal changes
    private bool _inBoard;
    private bool _isMine;
    
    // Actions & State
    private State _state;
    private float _stateTimer;
    private bool _canMakeAction;
    private bool _canAttack;

    private Action _onRevealComplete;
    private enum State
    {
        Flipping,
        Action,
        Attacking,
        CoolOff
    }

    // MonoBehaviour Callbacks **********
    private void Start()
    {
        _camera = Camera.main;

        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                if (!_isMine || !_isSelectable) return;

                if (_inBoard) AddToHand();
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
            case State.Action:
                if (_canMakeAction)
                {
                    Debug.Log("Ant action: add shield");
                    _canMakeAction = false;
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
                    _state = State.Action;
                    _stateTimer = 2f;
                    break;
                case State.Action:
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
        _isMine = GameManager.Instance.GetMyPlayer() == owner;

        artAnchor.sprite = _cardData.cardIcon;
    }
    public void TakeAction(Action onRevealComplete)
    {
        ActionStart(onRevealComplete);

        _state = State.Flipping;
        _stateTimer = 1f;

        _canMakeAction = true;
        _canAttack = true;
    }

    
    public void AddToBoard()
    {
        transform.position += ImOwner() ? Vector3.up * 2.5f : Vector3.up * -2.5f;
        _inBoard = true;
        
        _owner.RemoveFromHand(this);
        Board.Instance.AddToBoardHand(this, _owner);
    }
    
    public bool ImOwner() => _owner == GameManager.Instance.GetMyPlayer();

    public void Remove()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.75f).setEase(LeanTweenType.easeInBack).setDestroyOnComplete(true);
    }
    
    // Private Methods **********
    private void AddToHand()
    {
        transform.position += Vector3.down * 2.5f;
        _inBoard = false;
        
        _owner.AddToHand(this);
        Board.Instance.RemoveFromBoardHand(this);
    }

    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        _isSelectable = true;
    }
    
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        _isSelectable = false;
    }

    protected void MakeAttack()
    {
        LeanTween.moveY(gameObject,  GameManager.Instance.GetMyPlayer() == _owner ? transform.position.y + 0.25f: transform.position.y -0.25f, 0.25f).setEase(LeanTweenType.easeInBack).setLoopPingPong(1).setOnComplete(
            () =>
            {
                GameManager.Instance.TakeDamage(GameManager.Instance.GetEnemy(_owner), _cardData.attackPoints);
            });
    }

    protected void ActionStart(Action onRevealComplete)
    {
        _isActive = true;
        _onRevealComplete = onRevealComplete;
    }

    protected void ActionComplete()
    {
        Debug.Log((ImOwner() ? "Player " : "Enemy ") + "complete: " + _cardData.cardName);
        _isActive = false;
        _onRevealComplete();
    }
}
