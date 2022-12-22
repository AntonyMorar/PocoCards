using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    // MonoBehavior Callbacks
    private void OnEnable()
    {
        InputSystem.Instance.OnEscapeDown += InputSystem_OnEscapeDown;
    }

    private void OnDisable()
    {
        InputSystem.Instance.OnEscapeDown -= InputSystem_OnEscapeDown;
    }

    private void Start()
    {
        foreach (CardData cardData in GameManager.Instance.GetPlayerProfile().deck)
        {
            Debug.Log(cardData.cardName);
        }
    }

    // Private Methods
    private void InputSystem_OnEscapeDown(object sender, EventArgs e)
    {
        SceneManager.LoadScene(1);
    }
}
