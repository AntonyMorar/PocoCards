using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public int level;
    public int stones;
    public int maxHealth;
    public int levelCompleted;

    public PlayerProfile()
    {
        playerName = "Player 1";
        level = 1;
        stones = 0;
        maxHealth = 25;
        levelCompleted = 1;
    }
}