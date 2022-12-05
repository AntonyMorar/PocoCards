using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    // Serialized *****
    [SerializeField] private Card cardPrefab;
    [SerializeField] private int baseHealth = 300;
    [SerializeField] private Transform handTransform;
    [SerializeField] private List<CardData> deck = new List<CardData>();
    

    // Private *****
    private int _health;
    private int _coin;
    //Hand
    private List<Card> _hand = new List<Card>();

    
    // MonoBehaviour Callbacks *****
    private void Awake()
    {
        SetInitialValues();
    }
    private void Start()
    {
        UpdateStatusLog();
    }
    
    // Private Methods *****

    private void SetInitialValues()
    {
        _health = baseHealth;
    }
    
    private void UpdateStatusLog()
    {
        Debug.Log("Hola Jugador: Tu vida actual es: " + _health + ", Tus monedas: " + _coin);
    }
    

    private void ReorderCards()
    {
        int handSize = _hand.Count;
        float cardSize = 1f;
        int handHalf = handSize / 2;
        Debug.Log("Half: " + handHalf);
        for (int i=0; i<handSize; i++)
        {
            float newPosX = handSize % 2 == 0 ? i - handHalf + (cardSize/2) : i - handHalf;
            Vector3 position = handTransform.position;
            _hand[i].transform.position = new Vector3(position.x + newPosX, position.y, position.z);
        }
    }
    
    // Public Methods
    public void DrawCard()
    {
        if (deck.Count <= 0)
        {
            Debug.LogError("No cards in the Deck");
            return;
        }
        
        CardData randCardData = deck[Random.Range(0, deck.Count)];
        
        Card newCard = Instantiate(cardPrefab, handTransform);
        newCard.SetCard(randCardData, this);
        AddToHand(newCard);
        ReorderCards();
    }
    
    public void AddToHand(Card card)
    {
        _hand.Add(card);
    }
    public void RemoveFromHand(Card card)
    {
        _hand.Remove(card);
    }
    public int GetDeckSize() => deck.Count;
}
