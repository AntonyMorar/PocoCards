using System;
using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    // Public *****
    public static MatchManager Instance { get; private set; }
    public enum GamePhase
    {
        Idle,
        PreMain,
        Main,
        Waiting, // Player click battle waiting for enemy
        Battle,
        GameOver
    }
    public event EventHandler OnPreMainStart;
    public event EventHandler OnMainStart;
    public event EventHandler OnBattleStart;
    public event EventHandler OnWaitingStart; 
    public event EventHandler<bool> OnGameOver;
    public event EventHandler<int> OnTurnChange;
    public event EventHandler OnRestartGame;
    

    // Serialized *****
    [Header("Players")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemyPlayer;
    [Header("Match Preferences")]
    [SerializeField] private float initialDelay = 1.5f;
    [SerializeField] private float mainPhaseTime = 15f;
    [SerializeField] private int initialCards = 3;
    // Private *****
    private GamePhase _gamePhase;
    private float _phaseTimer;
    private int _turn;

    private DeckData _playerDeck;

    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        _phaseTimer = initialDelay;
    }
    
    private void Update()
    {
        if ( _gamePhase is GamePhase.PreMain or GamePhase.Battle or GamePhase.GameOver) return;
        
        _phaseTimer -= Time.deltaTime;

        if (_phaseTimer <= 0)
        {
            switch (_gamePhase)
            {
                case GamePhase.Idle:
                    SetPhase(GamePhase.PreMain);
                    StartCoroutine(SetInitialCards());
                    break;
                case GamePhase.Main:
                case GamePhase.Waiting:
                    SetPhase(GamePhase.Battle);
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
        SetPhase(GamePhase.Main);
    }

    // Public Methods *****
    public void SetPhase(GamePhase gamePhase)
    {
        if (_gamePhase == gamePhase) return;
        
        _gamePhase = gamePhase;
        switch (gamePhase)
        {
            case GamePhase.Idle:
                _turn = 0;
                _phaseTimer = initialDelay;
                break;
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
            case GamePhase.Waiting:
                OnWaitingStart?.Invoke(this,EventArgs.Empty);
                break;
            case GamePhase.Battle:
                OnBattleStart?.Invoke(this,EventArgs.Empty);
                break;
            case GamePhase.GameOver:
                OnGameOver?.Invoke(this,player.GetHealth() > 0);
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
    public GamePhase GetGamePhase() => _gamePhase;

    public bool TryStartBattle()
    {
        if (enemyPlayer.TryGetComponent(out PlayerAI playerAI) && !playerAI.IsSelectingCards())
        {
            SetPhase(GamePhase.Battle);
            return true;
        }
        
        SetPhase(GamePhase.Waiting);
        return false;
    }
    public void RestartGame()
    {
        SetPhase(GamePhase.Idle);
        OnRestartGame?.Invoke(this, EventArgs.Empty);
    }
}
