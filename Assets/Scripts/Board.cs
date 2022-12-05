using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Public
    public static Board Instance { get; private set; }
    
    // Private 
    private List<Card> _cardList = new List<Card>();

    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    // Public Methods *****
    public void AddToBoardHand(Card card)
    {
        _cardList.Add(card);
    }

    public void RemoveFromBoardHand(Card card)
    {
        _cardList.Remove(card);
    }

}
