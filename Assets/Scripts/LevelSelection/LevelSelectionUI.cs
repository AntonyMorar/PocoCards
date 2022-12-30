using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private Button[] levels;

    // MonoBehaviour Callbacks *****
    private void OnEnable()
    {
        UpdateUI();

        for (int i = 0; i < levels.Length; i++)
        {
            var i1 = i;
            levels[i].onClick.AddListener(() => AddButtonListener(i1));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            var i1 = i;
            levels[i].onClick.RemoveListener(() => AddButtonListener(i1));
        }
    }
    
    // Private Methods ****
    private void AddButtonListener(int i1)
    {
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.UiSelect);
                
        GameManager.Instance.SelectLevel(i1);
        LevelsManager.Instance.ChangeScene(GameManager.SceneState.InGame);
    }

    private void UpdateUI()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (GameManager.Instance.GetPlayerProfile().levelsAvailable[i])
            {
                levels[i].interactable = true;
            }
            else
            {
                levels[i].interactable = false;
            }
        }
    }
}
