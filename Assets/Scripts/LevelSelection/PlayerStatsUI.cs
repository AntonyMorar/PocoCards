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
        healthText.text = GameManager.Instance.GetPlayerProfile().maxHealth + "/" + GameManager.Instance.GetPlayerProfile().maxHealth;
    }
}
