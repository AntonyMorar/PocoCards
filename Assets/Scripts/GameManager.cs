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
    [SerializeField] private float initialDelay = 1.5f;
    [Header("Players")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemyPlayer;
    [Header("Hand")]
    [SerializeField] private int initialCards = 3;
    // Private *****
    private GamePhase _gamePhase;
    private float _gamePhaseTimer;
    private int _turn;
    
    // MonoBehavior Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        _gamePhaseTimer = initialDelay;
    }
    private void Update()
    {
        if (_gamePhase != GamePhase.Idle) return;
        
        _gamePhaseTimer -= Time.deltaTime;

        if (_gamePhaseTimer <= 0)
        {
            switch (_gamePhase)
            {
                case GamePhase.Idle:
                    StartPhase(GamePhase.Main);
                    StartCoroutine(SetInitialCards());
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
}
