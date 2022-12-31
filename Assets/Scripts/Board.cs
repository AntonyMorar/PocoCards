using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

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

    private void OnEnable()
    {
        MatchManager.Instance.OnBattleStart += GameManager_OnBattleStart;
        MatchManager.Instance.OnGameOver += GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame += GameManager_RestartGame;
    }

    private void OnDisable()
    {
        MatchManager.Instance.OnBattleStart -= GameManager_OnBattleStart;
        MatchManager.Instance.OnGameOver -= GameManager_OnGameOver;
        MatchManager.Instance.OnRestartGame -= GameManager_RestartGame;
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

        float width = _spriteRenderer.sprite.bounds.size.x;
        float height = _spriteRenderer.sprite.bounds.size.y;

        double worldScreenHeight = GameManager.Instance.GetCamera().orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var localScale = new Vector3((float)worldScreenWidth / width, (float)(worldScreenHeight / height)* 0.4f, 0);
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
        MatchManager.Instance.SetPhase(MatchManager.GamePhase.Main);
    }

    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        _handToPlay.Clear();
        
        if (_hand.Count <= 0)
        {
            StartCoroutine(SetMainCorrutine());
            return;
        }
        
        foreach (Card card in _hand)
        {
            _handToPlay.Add(card);
        }
        _inBattlePhase = true;
        _waitingBattlePhaseEnd = false;
    }

    private IEnumerator SetMainCorrutine()
    {
        yield return new WaitForSeconds(0.1f);
        MatchManager.Instance.SetPhase(MatchManager.GamePhase.Main);
    }

    private void GameManager_OnGameOver(object sender, bool won)
    {
        RestartValues();
    }

    private void GameManager_RestartGame(object sender, EventArgs e)
    {
        RestartValues();
    }

    private void RestartValues()
    {
        _isBusy = false;
        _inBattlePhase = false;
        _waitingBattlePhaseEnd = false;
        
        _hand.Clear();
        _handToPlay.Clear();
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

    private Vector3 GetCardPositionInHand(List<Card> hand, int index, int newCardsToAdd)
    {
        float cardWidth = 1.25f;
        float spacing = 0.1f;
        float totalWidth = ((hand.Count + newCardsToAdd) * cardWidth) + ((hand.Count + newCardsToAdd) * cardWidth ) * spacing -spacing;
        Vector3 pivotOffset = new Vector3(cardWidth/2, 0, 0);

        Vector3 startingPosition = Vector3.zero;
        startingPosition.x = -totalWidth / 2f;

        return startingPosition + Vector3.right * index * (spacing + cardWidth) + pivotOffset;
    }

    private List<Card> GetPlayerCards(Player target)
    {
        List<Card> playerCards = new List<Card>();

        foreach (Card card in _hand)
        {
            if (card.GetOwner() == target)
            {
                playerCards.Add(card);
            }
        }
        return playerCards;
    }

    // Public Methods *****
    public void AddToHand(Card card, bool isNew = false)
    {
        ReorderActualCards(card.ImOwner() ? 1: 0, card.ImOwner() ? 0: 1);
        card.transform.SetParent(card.ImOwner() ? handAnchorPlayer : handAnchorEnemy);
        
        if (isNew) card.transform.position = new Vector3(0, card.ImOwner() ? -6 : 6, 0);

        List<Card> playerHand = new List<Card>();
        List<Card> enemyHand = new List<Card>();
        foreach (Card handCard in _hand)
        {
            if (handCard.ImOwner()) playerHand.Add(handCard);
            else enemyHand.Add(handCard);
        }
        LeanTween.moveLocal(card.gameObject, GetCardPositionInHand(card.ImOwner() ? playerHand : enemyHand, card.ImOwner() ? playerHand.Count : enemyHand.Count,1), 0.15f);

        _hand.Add(card);
    }
    
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
        ReorderActualCards(0,0);
    }

    private void ReorderActualCards(int newPlayerCardsToAdd, int newEnemyCardsToAdd)
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
            LeanTween.moveLocal(playerCards[i].gameObject, GetCardPositionInHand(playerCards, i, newPlayerCardsToAdd), 0.15f);
        }
        
        for (int i=0; i<enemyCards.Count; i++)
        {
            LeanTween.moveLocal(enemyCards[i].gameObject, GetCardPositionInHand(enemyCards, i, newEnemyCardsToAdd), 0.15f);
        }
        
        playerCards.Clear();
        enemyCards.Clear();
    }

    public void ChangeRandomCard(Player target, CardData cardData)
    {
        List<Card> playerCards = GetPlayerCards(target);
        if (playerCards.Count <= 0) return;
        
        int randomCard = Random.Range(0, playerCards.Count);
        playerCards[randomCard].ChangeCard(target, cardData);
    }
}
