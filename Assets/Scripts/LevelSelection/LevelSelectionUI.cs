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
    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (i < GameManager.Instance.GetPlayerProfile().levelCompleted)
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
