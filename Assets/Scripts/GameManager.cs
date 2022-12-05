using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Public *****
    public static GameManager Instance { get; private set; }
    public enum GamePhase
    {
        Idle,
        Selecting,
        Battle
    }
    public event EventHandler<GamePhase> OnGamePhaseChange;
    
    // Serialized *****
    [Header("Players")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemyPlayer;
    [Header("Hand")]
    [SerializeField] private int initialCards = 3;
    [Header("UI")]
    [SerializeField]private TMP_Text deckSizeText;
    [SerializeField]private TMP_Text enemyDeckSizeText;
    // Private *****
    private GamePhase _gamePhase;
    
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
    }

    private void Update()
    {
        deckSizeText.text = player.GetDeckSize().ToString();
        enemyDeckSizeText.text = enemyPlayer.GetDeckSize().ToString();
    }
    
    // Public Methods
    public void SetSlotAvailable(bool[] availableCardSlots, int index)
    {
        availableCardSlots[index] = true;
    }

    public void StartPhase(GamePhase gamePhase)
    {
        if (_gamePhase == gamePhase) return;
        
        _gamePhase = gamePhase;
        OnGamePhaseChange?.Invoke(this, gamePhase);
    }
    

    public Player GetMyPlayer() => player;
}
