using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class PlayerUI : MonoBehaviour
{
    // Serilized *****
    [SerializeField] private Player player;
    [SerializeField] private GameObject healthVfxPrefab;
    [SerializeField] private TMP_Text healthChangePrefab;
    //[SerializeField] private ParticleSystem 
    [Header("UI Reference")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text coinsText;

    private Camera _cam;
    // MonoBehaviour Callbacks *****

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        
        player.OnHealthChange += Player_OnHealthChange;
        player.OnRestoreHealth += Player_OnRestoreHealth;
        player.OnBalanceChange += Player_OnBalanceChange;
    }

    private void OnDisable()
    {
        player.OnHealthChange -= Player_OnHealthChange;
        player.OnRestoreHealth -= Player_OnRestoreHealth;
        player.OnBalanceChange -= Player_OnBalanceChange;
    }
    
    // Private Methods *****
    private void Player_OnHealthChange(object sender, Player.OnHealthChangeEventArgs healthArgs)
    {
        healthText.text = "Health: " + healthArgs.NewHealth;
        
        if (!healthArgs.ApplyEffects) return;
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_cam, healthText.transform.position);
        Vector2 worldPos = _cam.ScreenToWorldPoint(screenPosition); 
        Instantiate(healthChangePrefab, worldPos, quaternion.identity, healthText.transform);
    }

    private void Player_OnRestoreHealth(object sender, int health)
    {
        // Obtén la posición del elemento de UI en la pantalla
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_cam, healthText.transform.position);
        Vector2 worldPos = _cam.ScreenToWorldPoint(screenPosition); 
        GameObject healthVfx = Instantiate(healthVfxPrefab, worldPos, Quaternion.identity);
        Destroy(healthVfx, 4);
    }

    private void Player_OnBalanceChange(object sender, int coins)
    {
        coinsText.text = "Coins: " + coins.ToString();
    }
    
}
