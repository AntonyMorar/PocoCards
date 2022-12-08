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
        GameManager.Instance.OnMainStart += GameManager_OnMainStart;
        GameManager.Instance.OnBattleStart += GameManager_OnBattleStart;
        GameManager.Instance.OnTurnChange += GameManager_OnTurnChange;
        gamePhaseButton.onClick.AddListener(() => GameManager.Instance.StartPhase(GameManager.GamePhase.Battle));
    }

    // Private Methods *****
    private void GameManager_OnMainStart(object sender, EventArgs e)
    {
        gamePhaseText.text = "Main Phase";
        gamePhaseButton.enabled = true;
        gamePhaseButton.GetComponentInChildren<TMP_Text>().text = "Next Turn";
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
