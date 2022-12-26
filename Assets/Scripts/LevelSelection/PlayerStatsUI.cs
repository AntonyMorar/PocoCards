using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text healthText;
    private void Start()
    {
        levelText.text = "Level " + GameManager.Instance.GetPlayerProfile().level;
        int baseHealth = GameManager.Instance.GetPlayerProfile().baseHealth;
        healthText.text = baseHealth + "/" + baseHealth;
    }
}
