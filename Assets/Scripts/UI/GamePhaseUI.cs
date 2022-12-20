using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamePhaseUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private TMP_Text gamePhaseText;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private Button gamePhaseButton;

    // MonoBehavior Callbacks *****
    private void Start()
    {
        MatchManager.Instance.OnPreMainStart += GameManager_OnPreMainStart;
        MatchManager.Instance.OnMainStart += GameManager_OnMainStart;
        MatchManager.Instance.OnBattleStart += GameManager_OnBattleStart;
        MatchManager.Instance.OnTurnChange += GameManager_OnTurnChange;
        gamePhaseButton.onClick.AddListener(() => MatchManager.Instance.TryStartBattle());
    }

    // Private Methods *****
    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        gamePhaseText.text = "Main Phase";
        gamePhaseButton.enabled = true;
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "Next Turn";
    }
    
    private void GameManager_OnPreMainStart(object sender, EventArgs e)
    {
        gamePhaseText.text = "Spreading";
        gamePhaseButton.enabled = false;
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "Playing";
    }
    private void GameManager_OnBattleStart(object sender, EventArgs e)
    {
        gamePhaseText.text = "In Battle";
        gamePhaseButton.enabled = false;
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "Playing";
    }

    private void GameManager_OnTurnChange(object sender, int newTurn)
    {
        turnText.text = "Turn: " + newTurn;
    }
    private void ChangeState()
    {
        Debug.Log("CHANGE STATE");
        
    }

}
