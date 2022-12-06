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
    
    // Private *****
    private List<Card> _handBoard = new List<Card>();
    private List<Card> _handToPlay = new List<Card>();
    private bool _isBusy;
    private bool _inBattlePhase;

    private int _allyCards;
    private int _enemyCards;

    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
    }

    private void Update()
    {
        if (!_inBattlePhase || _isBusy) return;

        if (_handToPlay.Count <= 0)
        {
            _inBattlePhase = false;
            
            foreach (Card card in _handBoard)
            {
                card.Remove();
            }
            _handToPlay.Clear();
            _handBoard.Clear();
            _allyCards = 0;
            _enemyCards = 0;
            GameManager.Instance.StartPhase(GameManager.GamePhase.Main);
            return;
        }
        
        SetBusy();
        _handToPlay[0].TakeAction(ClearBusy);
    }

    // Private Methods *****
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        foreach (Card card in _handBoard)
        {
            _handToPlay.Add(card);
        }
        _inBattlePhase = true;
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
    public void AddToBoardHand(Card card, Player owner)
    {
        _handBoard.Add(card);
        if (owner == GameManager.Instance.GetMyPlayer())
        {
            _allyCards++;
        }
        else
        {
            _enemyCards++;
        }
    }

    public void RemoveFromBoardHand(Card card)
    {
        _handBoard.Remove(card);
    }

}
