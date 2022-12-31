using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamePhaseUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private Button gamePhaseButton;
    [SerializeField] private TMP_Text gamePhaseText;

    // MonoBehavior Callbacks *****
    private void Awake()
    {
        GamePhaseInteractable(false);
    }

    private void Start()
    {
        MatchManager.Instance.OnMainStart += GameManager_OnMainStart;
        MatchManager.Instance.OnWaitingStart += GameManager_OnPreBattleStart;
        MatchManager.Instance.OnPreBattleStart += GameManager_OnPreBattleStart;
        gamePhaseButton.onClick.AddListener(() => MatchManager.Instance.TryStartBattle());
    }

    // Private Methods *****
    private void GameManager_OnMainStart(object sender, int newTurn)
    {
        GamePhaseInteractable(true);
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "Next Turn";
        
        turnText.text = "Turn: " + newTurn;
    }
    private void GameManager_OnPreBattleStart(object sender, EventArgs e)
    {
        GamePhaseInteractable(false);
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "OnBattle";
    }

    private void GamePhaseInteractable(bool interactable)
    {
        gamePhaseText.alpha = interactable ? 1 : 0.5f;
        gamePhaseButton.interactable = interactable;
    }
}
