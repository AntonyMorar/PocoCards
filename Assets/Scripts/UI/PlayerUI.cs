using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    // Serilized *****
    [SerializeField] private Player player;
    [Header("UI")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text coinsText;

    // MonoBehaviour Callbacks *****
    private void OnEnable()
    {
        player.OnHealthChange += Player_OnHealthChange;
        player.OnBalanceChange += Player_OnBalanceChange;
    }

    private void OnDisable()
    {
        player.OnHealthChange += Player_OnHealthChange;
        player.OnBalanceChange -= Player_OnBalanceChange;
    }
    
    // Private Methods *****
    private void Player_OnHealthChange(object sender, int health)
    {
        healthText.text = "Health: " + health.ToString();
    }
    
    private void Player_OnBalanceChange(object sender, int coins)
    {
        coinsText.text = "Coins: " + coins.ToString();
    }
}
