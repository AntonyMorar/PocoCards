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
    private float _selectingTime;
    private float _actualSelectingTime;
    private List<Card> _cardsCanBuy = new List<Card>();
    
    private float _stepTime;
    private int _totalSteps;

    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnWaitingStart += GameManager_OnWaitingStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMainStart -= GameManager_OnMainStart;
        GameManager.Instance.OnWaitingStart -= GameManager_OnWaitingStart;
        GameManager.Instance.OnBattleStart -= GameManager_OnBattleStart;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void Update()
    {
        SelectingCardsUpdate();
    }

    // Private Methods *****
    private void GameManager_OnMainStart(object sender,EventArgs e)
    {
        RestartValues();
        _cardsCanBuy = GetCardsCanBuy();
        if (_cardsCanBuy.Count <= 0) return;
        
        _isSelectingCards = true;
        
        if(_cardsCanBuy.Count == 1) _selectingTime = Random.Range(GameManager.Instance.GetMainPhaseTime() * 0.05f, GameManager.Instance.GetMainPhaseTime() * 0.4f);
        else _selectingTime = Random.Range(GameManager.Instance.GetMainPhaseTime() * 0.15f, GameManager.Instance.GetMainPhaseTime() * 0.8f);
        _actualSelectingTime = _selectingTime;
        
        _stepTime = _selectingTime / _cardsCanBuy.Count;
        _totalSteps = _cardsCanBuy.Count -1;
    }

    private void GameManager_OnWaitingStart(object sender,EventArgs e)
    {
        if (!_isSelectingCards)
        {
            GameManager.Instance.SetPhase(GameManager.GamePhase.Battle);
        }
        else
        {
            if (_actualSelectingTime > 2)
            {
                _actualSelectingTime = Random.Range(0, 2);
            }
        }
    }

    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        RestartValues();
    }

    private void GameManager_OnGameOver(object sender,bool won)
    {
        RestartValues();
    }

    private void RestartValues()
    {
        _isSelectingCards = false;
        _selectingTime = GameManager.Instance.GetMainPhaseTime();
        _actualSelectingTime = _selectingTime;
        _stepTime = _selectingTime;
        _cardsCanBuy.Clear();
    }
    
    private void SelectingCardsUpdate()
    {
        if (!_isSelectingCards) return;
        _actualSelectingTime -= Time.deltaTime;

        if (_actualSelectingTime < _stepTime * _totalSteps && _cardsCanBuy.Count > 0)
        {
            _cardsCanBuy[0].AddToBoard();
            _cardsCanBuy.RemoveAt(0);
            _totalSteps--;
        }
        
        if (_actualSelectingTime <= 0)
        {
            _cardsCanBuy.Clear();
            _isSelectingCards = false;

            if (GameManager.Instance.GetGamePhase() == GameManager.GamePhase.Waiting)
                GameManager.Instance.SetPhase(GameManager.GamePhase.Battle);
        }
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
