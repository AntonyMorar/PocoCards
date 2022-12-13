using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Board : MonoBehaviour, IHandeable
{
    // Public *****
    public static Board Instance { get; private set; }
    public event EventHandler<bool> OnBusyChange;
    
    // Serialized *****
    [SerializeField] private Transform handAnchorPlayer;
    [SerializeField] private Transform handAnchorEnemy;
    
    // Private *****
    private SpriteRenderer _spriteRenderer;
    private List<Card> _hand = new List<Card>();
    private List<Card> _handToPlay = new List<Card>();
    private bool _isBusy;
    private bool _inBattlePhase;
    private bool _waitingBattlePhaseEnd;

    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        ResizeBoardToScreen();
    }

    private void Start()
    {
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    private void Update()
    {
        if (!_inBattlePhase ||  _waitingBattlePhaseEnd || _isBusy) return;

        if (_handToPlay.Count <= 0)
        {
            _waitingBattlePhaseEnd = true;
            StartCoroutine(RemoveCardsCorrutine(1));
            return;
        }
        
        SetBusy();
        _handToPlay[0].TakeAction(ClearBusy);
    }

    // Private Methods *****
    private void ResizeBoardToScreen()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (!_spriteRenderer) return;

        var localScale = transform.localScale;
        localScale = Vector3.one;
        
        float width = _spriteRenderer.sprite.bounds.size.x;
        float height = _spriteRenderer.sprite.bounds.size.y;

        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        localScale = new Vector3((float)worldScreenWidth / width, (float)(worldScreenHeight / height)* 0.4f, 0);
        transform.localScale = localScale;
    }
    private IEnumerator RemoveCardsCorrutine(int delay)
    {
        yield return new WaitForSeconds(delay);
        
        foreach (Card card in _hand)
        {
            card.Remove();
        }
        
        _inBattlePhase = false;
        _handToPlay.Clear();
        _hand.Clear();
        GameManager.Instance.StartPhase(GameManager.GamePhase.Main);
    }

    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        foreach (Card card in _hand)
        {
            _handToPlay.Add(card);
        }
        _inBattlePhase = true;
        _waitingBattlePhaseEnd = false;
    }

    private void SetBusy()
    {
        _isBusy = true;
        OnBusyChange?.Invoke(this,_isBusy);
    }
    
    private void ClearBusy()
    {
        _handToPlay.RemoveAt(0);
        _isBusy = false;
        OnBusyChange?.Invoke(this,_isBusy);
    }

    // Public Methods *****
    public void AddToHand(Card card)
    {
        _hand.Add(card);
        card.transform.SetParent(card.ImOwner() ? handAnchorPlayer : handAnchorEnemy, false);
        
        ReorderCards();
    }
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderCards();
    }
    public void ReorderCards()
    {
        if(_hand.Count <= 0) return;
            
        List<Card> playerCards = new List<Card>();
        List<Card> enemyCards = new List<Card>();
        
        foreach (Card card in _hand)
        {
            if (card.ImOwner()) playerCards.Add(card);
            else enemyCards.Add(card);
        }
        
        // Card Settings
        float cardSize = 1.25f;
        int handHalfPlayer = playerCards.Count / 2;
        int handHalfEnemy = enemyCards.Count / 2;

        for (int i=0; i<playerCards.Count; i++)
        {
            float newPosX = playerCards.Count % 2 == 0 ? i*cardSize - handHalfPlayer + (cardSize/2) : i*cardSize - handHalfPlayer;
            playerCards[i].transform.localPosition = new Vector3(newPosX, 0f, 0f);
        }
        
        for (int i=0; i<enemyCards.Count; i++)
        {
            float newPosX = enemyCards.Count % 2 == 0 ? i*cardSize - handHalfEnemy + (cardSize/2) : i*cardSize - handHalfEnemy;
            enemyCards[i].transform.localPosition = new Vector3(newPosX, 0f, 0f);
        }
        
        playerCards.Clear();
        enemyCards.Clear();
    }
}
