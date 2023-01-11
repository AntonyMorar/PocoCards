using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    // Public *****
    public static EventHandler OnEscDeck;
    public static EventHandler OnOpenSettings;
    public static EventHandler OnCloseSettings;
    public static EventHandler OnQ;

    // MonoBehavior Callbacks ****
    void Update()
    {
        ScapeDown();
        QDown();
    }

    // Private Methods*****
    private void ScapeDown()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (GameManager.Instance.GetState() == GameManager.SceneState.DeckEditor)
        {
            OnEscDeck?.Invoke(this, EventArgs.Empty);
        }
        else if(GameManager.Instance.GetState() == GameManager.SceneState.MainMenu || GameManager.Instance.GetState() == GameManager.SceneState.InGame)
        {
            OnOpenSettings?.Invoke(this, EventArgs.Empty);
        }
    }

    private void QDown()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQ?.Invoke(this, EventArgs.Empty);
        }
    }
}
