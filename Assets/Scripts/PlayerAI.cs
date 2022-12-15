using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Player))]
public class PlayerAI : MonoBehaviour
{
    // Private
    private Player _player;
    private bool _isSelectingCards;
    private IEnumerator _selectingCardsCorrutine;
    
    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMainStart -= GameManager_OnMainStart;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }
    
    // Private Methods *****
    private void GameManager_OnMainStart(object sender,EventArgs e)
    {
        _isSelectingCards = true;

        _selectingCardsCorrutine = SelectCardsToPlay();
        StartCoroutine(_selectingCardsCorrutine);
    }
    private void GameManager_OnGameOver(object sender,EventArgs e)
    {
        _isSelectingCards = false;
        StopCoroutine(_selectingCardsCorrutine);
    }

    private IEnumerator SelectCardsToPlay()
    {
        float timeThinking = Random.Range(GameManager.Instance.GetMainPhaseTime() * 0.1f, GameManager.Instance.GetMainPhaseTime() * 0.8f);
        List<Card> cardsCanBuy = GetCardsCanBuy();
        
        if (cardsCanBuy.Count <= 0) yield return null;
        if (cardsCanBuy.Count == 1) timeThinking = 2f;
        
        foreach (Card card in cardsCanBuy)
        {
            yield return new WaitForSeconds(timeThinking / cardsCanBuy.Count);
            card.AddToBoard();
        }

        _isSelectingCards = false;
        if(GameManager.Instance.GetGamePhase() == GameManager.GamePhase.Waiting) GameManager.Instance.SetPhase(GameManager.GamePhase.Battle);
    }
    
    
    // TODO: Change this function to improve the AI
    public List<Card> GetCardsCanBuy()
    {
        List<Card> cardsCanBuy = new List<Card>();
        int coinsHelper = _player.GetCoins();
        foreach (Card card in _player.GetHand())
        {
            if (card.IsNotSelectableOrFrozen()) continue;
            
            if (card.GetActualCost() <= coinsHelper)
            {
                cardsCanBuy.Add(card);
                coinsHelper -= card.GetActualCost();
            }
        }

        return cardsCanBuy;
    }

    public bool IsSelectingCards() => _isSelectingCards;
}
