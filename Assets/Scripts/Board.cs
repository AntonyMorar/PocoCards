using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Board : MonoBehaviour
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
            StartCoroutine(RemoveCardsCorrutine(0.5f));
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
    private IEnumerator RemoveCardsCorrutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        foreach (Card card in _hand)
        {
            yield return new WaitForSeconds(0.15f);
            card.Remove();
        }
        
        _inBattlePhase = false;
        _handToPlay.Clear();
        _hand.Clear();
        GameManager.Instance.SetPhase(GameManager.GamePhase.Main);
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
    public void AddToHand(Card card, bool isNew = false)
    {
        ReorderActualCards(card.ImOwner() ? 1: 0, card.ImOwner() ? 0: 1);
        card.transform.SetParent(card.ImOwner() ? handAnchorPlayer : handAnchorEnemy);
        
        if (isNew) card.transform.position = new Vector3(0, card.ImOwner() ? -6 : 6, 0);
        LeanTween.moveLocal(card.gameObject, GetCardPositionInHand(_hand.Count,1), 0.15f);

        _hand.Add(card);
    }
    
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderActualCards(0,0);
    }

    public void ReorderActualCards(int newPlayerCardsToAdd, int newEnemyCardsToAdd)
    {
        List<Card> playerCards = new List<Card>();
        List<Card> enemyCards = new List<Card>();
        
        foreach (Card card in _hand)
        {
            if (card.ImOwner()) playerCards.Add(card);
            else enemyCards.Add(card);
        }
        
        for (int i=0; i<playerCards.Count; i++)
        {
            LeanTween.moveLocal(playerCards[i].gameObject, GetCardPositionInHand(i, newPlayerCardsToAdd), 0.15f);
        }
        
        for (int i=0; i<enemyCards.Count; i++)
        {
            LeanTween.moveLocal(enemyCards[i].gameObject, GetCardPositionInHand(i, newEnemyCardsToAdd), 0.15f);
        }
        
        playerCards.Clear();
        enemyCards.Clear();
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
}
