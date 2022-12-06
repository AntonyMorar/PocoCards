using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    // Serialized **********
    [SerializeField] private LayerMask layer;
    [SerializeField] protected CardData cardData;
    [Header("References")] 
    [SerializeField] private SpriteRenderer icon;
    // Private **********
    private Camera _camera;
    private bool _isSelectable = true;
    protected Player _owner;
    protected bool _isActive; // Making on reveal changes
    private bool _inBoard;
    private bool _isMine;

    private Action _onRevealComplete;
    protected enum State
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
    }
    

    // Public Methods **********
    public abstract void TakeAction(Action onRevealComplete);
    public void SetCard(Player owner)
    {
        _owner = owner;
        _isMine = GameManager.Instance.GetMyPlayer() == owner;

        icon.sprite = cardData.cardIcon;
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
                GameManager.Instance.TakeDamage(GameManager.Instance.GetEnemy(_owner), cardData.attackPoints);
            });
    }

    protected void ActionStart(Action onRevealComplete)
    {
        _isActive = true;
        _onRevealComplete = onRevealComplete;
    }

    protected void ActionComplete()
    {
        Debug.Log((ImOwner() ? "Player " : "Enemy ") + "complete: " + cardData.cardName);
        _isActive = false;
        _onRevealComplete();
    }
}
