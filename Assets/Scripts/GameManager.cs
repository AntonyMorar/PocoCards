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
        PreMain,
        Main,
        Battle,
        GameOver
    }
    public event EventHandler OnMainStart;
    public event EventHandler OnPreMainStart;
    public event EventHandler OnBattleStart;
    public event EventHandler OnGameOver;
    public event EventHandler<int> OnTurnChange;

    // Serialized *****
    [SerializeField] private float initialDelay = 1.5f;
    [SerializeField] private float mainPhaseTime = 15f;
    [Header("Players")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemyPlayer;
    [Header("Hand")]
    [SerializeField] private int initialCards = 3;
    // Private *****
    private GamePhase _gamePhase;
    private float _phaseTimer;
    private int _turn;
    
    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        _phaseTimer = initialDelay;
    }
    private void Update()
    {
        if ( _gamePhase is GamePhase.PreMain or GamePhase.Battle) return;
        
        _phaseTimer -= Time.deltaTime;

        if (_phaseTimer <= 0)
        {
            switch (_gamePhase)
            {
                case GamePhase.Idle:
                    StartPhase(GamePhase.PreMain);
                    StartCoroutine(SetInitialCards());
                    break;
                case GamePhase.Main:
                    StartPhase(GamePhase.Battle);
                    break;
            }
        }
    }

    // Private Methods *****
    private IEnumerator SetInitialCards()
    {
        for (int i = 0; i < initialCards; i++)
        {
            player.DrawCard();
            enemyPlayer.DrawCard();
            yield return new WaitForSeconds(0.33f);
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
            case GamePhase.PreMain:
                OnPreMainStart?.Invoke(this,EventArgs.Empty);
                break;
            case GamePhase.Main:
                _turn++;
                _phaseTimer = mainPhaseTime;
                
                if (_turn > 1)
                {
                    player.DrawCard();
                    enemyPlayer.DrawCard();
                }
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

    // TODO: Put this function in player
    public void TakeDamage(Player target, int damage)
    {
        target.TakeDamage(damage);
    }

    public float GetMainPhaseTime() => mainPhaseTime;
}
