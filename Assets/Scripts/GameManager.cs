using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Public *****
    public static GameManager Instance { get; private set; }
    public enum GamePhase
    {
        Idle,
        Main,
        Battle,
        GameOver
    }
    public event EventHandler OnMainStart;
    public event EventHandler OnBattleStart;
    public event EventHandler OnGameOver;
    public event EventHandler<int> OnTurnChange;

    // Serialized *****
    [Header("Players")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemyPlayer;
    [Header("Hand")]
    [SerializeField] private int initialCards = 3;
    // Private *****
    private GamePhase _gamePhase;
    private int _turn;
    
    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < initialCards; i++)
        {
            player.DrawCard();
            enemyPlayer.DrawCard();
        }
        
        StartPhase(GamePhase.Main);
    }

    // Public Methods *****
    public void StartPhase(GamePhase gamePhase)
    {
        if (_gamePhase == gamePhase) return;
        
        _gamePhase = gamePhase;
        switch (gamePhase)
        {
            case GamePhase.Main:
                _turn++;
                
                player.DrawCard();
                enemyPlayer.DrawCard();
                
                OnTurnChange?.Invoke(this, _turn);
                OnMainStart?.Invoke(this,EventArgs.Empty);
                break;
            case GamePhase.Battle:
   
                OnBattleStart?.Invoke(this,EventArgs.Empty);
                break;
            case GamePhase.GameOver:
                OnGameOver?.Invoke(this,EventArgs.Empty);
                break;
        }
    }
    public Player GetMyPlayer() => player;
    /// <summary>
    /// Return the contrary player
    /// </summary>
    /// <param name="owner">Put your owner to get your enemy</param>
    /// <returns></returns>
    public Player GetEnemy(Player owner) => owner == player ? enemyPlayer : player;

    public void TakeDamage(Player target, int damage)
    {
        target.TakeDamage(damage);
    }
}
