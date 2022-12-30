using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    // Public *****
    public static InputSystem Instance { get; private set; }
    
    public static EventHandler OnOpenSettings;
    public static EventHandler OnCloseSettings;
    public static EventHandler<BaseCard> OnCardClick;

    // MonoBehavior Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Update()
    {
        SelectingCards();
        ScapeDown();
    }

    // Private Methods*****
    private void ScapeDown()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (GameManager.Instance.GetState() == GameManager.SceneState.DeckEditor)
        {
            //Audio
            SoundManager.PlaySound(SoundManager.Sound.UiSelect);
            GameManager.Instance.Save();
            LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
        }
        else if(GameManager.Instance.GetState() == GameManager.SceneState.MainMenu || GameManager.Instance.GetState() == GameManager.SceneState.InGame)
        {
            OnOpenSettings?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SelectingCards()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(GameManager.Instance.GetCamera().ScreenPointToRay(Input.mousePosition),50);

            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out BaseCard baseCard))
            {
                OnCardClick?.Invoke(this,baseCard );
            }
        }
    }

}
