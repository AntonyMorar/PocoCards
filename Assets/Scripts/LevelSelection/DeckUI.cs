using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUI : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(SaveData.current.profile.playerName);
    }
}
