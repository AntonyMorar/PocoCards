using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private Player player;
    [SerializeField] private Canvas mainCanvas;

    [Header("UI Refernece")] 
    [SerializeField] private Image profilePic;
    [SerializeField] private Image imageHealthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private RectTransform coinsContainer;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private PlayerEffectUI playerEffect;
    [Header("Health")]
    [SerializeField] private GameObject healthVfxPrefab;
    [SerializeField] private TMP_Text floatingHealthPrefab;
    [Header("Poison")]
    [SerializeField] private GameObject poisonPrefab;

    private Camera _cam;
    // MonoBehaviour Callbacks *****

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        player.OnSetUpComplete += Player_OnSetUpComplete;
        
        player.OnHealthChange += Player_OnHealthChange;
        player.OnRestoreHealth += Player_OnRestoreHealth;
        // Poison
        player.OnPoisonAdd += Player_OnPoisonAdd;
        player.OnPoisonDamage += Player_OnPoisonDamage;
        player.OnPoisonRemoved += Player_OnPoisonRemoved;
        // Balance
        player.OnBalanceChange += Player_OnBalanceChange;
        player.OnCoinStealed += Player_OnCoinStealed;
    }

    private void OnDisable()
    {
        player.OnSetUpComplete += Player_OnSetUpComplete;
        
        player.OnHealthChange -= Player_OnHealthChange;
        player.OnRestoreHealth -= Player_OnRestoreHealth;
        // Poison
        player.OnPoisonAdd -= Player_OnPoisonAdd;
        player.OnPoisonDamage -= Player_OnPoisonDamage;
        player.OnPoisonRemoved -= Player_OnPoisonRemoved;
        // Balance
        player.OnBalanceChange -= Player_OnBalanceChange;
        player.OnCoinStealed += Player_OnCoinStealed;
        
    }
    
    // Private Methods *****
    private void Player_OnSetUpComplete(object sender, EventArgs e)
    {
        Debug.Log("UI SETUP: " + player.GetProfilePic());
        profilePic.sprite = player.GetProfilePic();
    }
    private void Player_OnHealthChange(object sender, Player.OnHealthChangeEventArgs healthArgs)
    {
        healthText.text = healthArgs.NewHealth + "/" + player.GetBaseHealth();
        imageHealthBar.fillAmount = healthArgs.NewHealth / player.GetBaseHealth();
        
        if (!healthArgs.ApplyEffects) return;
        
        TMP_Text floatingHealth = Instantiate(floatingHealthPrefab, GetSpawnPosition(true), quaternion.identity, mainCanvas.transform);
        floatingHealth.text = healthArgs.Amountchange > 0 ? "+" + healthArgs.Amountchange : healthArgs.Amountchange.ToString();
    }

    private void Player_OnRestoreHealth(object sender, int health)
    {
        // Obtén la posición del elemento de UI en la pantalla
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_cam, healthText.transform.position);
        Vector2 worldPos = _cam.ScreenToWorldPoint(screenPosition); 
        GameObject healthVfx = Instantiate(healthVfxPrefab, worldPos, Quaternion.identity);
        Destroy(healthVfx, 4);
    }

    // Balance
    private void Player_OnBalanceChange(object sender, int coins)
    {
        coinsText.text = coins.ToString();
        LeanTween.scale(coinsContainer, new Vector3(1.2f, 1.2f, 1.2f), 0.12f)
            .setLoopPingPong(1);
    }
    
    private void Player_OnCoinStealed(object sender, int amount)
    {
        TMP_Text floatingCoins = Instantiate(floatingHealthPrefab, GetSpawnPosition(false), quaternion.identity, mainCanvas.transform);
        floatingCoins.color = new Color(255, 180, 0, 255);
        floatingCoins.text = "+" + amount + " C";
    }

    // Poison
    private void Player_OnPoisonAdd(object sender, int poisonedAmount)
    {
        Instantiate(poisonPrefab, GetSpawnPosition(true), quaternion.identity, mainCanvas.transform);
        playerEffect.gameObject.SetActive(true);
        playerEffect.Set(PlayerEffectUI.Effect.Poisoned, poisonedAmount);
    }

    private void Player_OnPoisonDamage(object sender, int poisonedAmount)
    {
        playerEffect.UpdateAmount(poisonedAmount);
    }

    private void Player_OnPoisonRemoved(object sender, EventArgs e)
    {
        playerEffect.gameObject.SetActive(false);
    }
    
    // Spawn
    private Vector2 GetSpawnPosition(bool healthTarget)
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_cam, healthTarget ? healthText.transform.position : coinsText.transform.position);
        return _cam.ScreenToWorldPoint(screenPosition); 
    }
}
