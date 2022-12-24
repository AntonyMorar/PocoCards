using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private Transform origin;
    [Tooltip("Max number of cards in hand")]
    [SerializeField] private int maxCards = 6;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private BaseCard cardPrefab;
    [SerializeField] private Constrain constraint;
    [SerializeField] private int constraintCount;
    // Private ****
    private enum Constrain
    {
        Flexible,
        FixedColumCount
    }
    private List<BaseCard> _cards = new List<BaseCard>();
    
    // Public Methods ****
    public bool AddCard(CardData cardData)
    {
        if (_cards.Count >= maxCards) return false;
        
        BaseCard newCard = Instantiate(cardPrefab, transform);
        newCard.SetCard(cardData, true);
        _cards.Add(newCard);
        OrderCards();
        return true;
    }

    public bool RemoveCard(BaseCard card)
    {
        if (_cards.Count <= 0) return false;
        
        _cards.Remove(card);
        Destroy(card.gameObject);
        OrderCards();
        return true;
    }

    public List<BaseCard> GetCards() => _cards;

    // Private Methods *****
    private void OrderCards()
    {
        float cardWidth = 1;
        int maxCol = constraint == Constrain.Flexible ? 99 : constraintCount;
        int cols = constraint == Constrain.Flexible ? _cards.Count : Mathf.Clamp(_cards.Count, 0, maxCol);
        Vector3 pivotOffset = new Vector3(cardWidth/2, 0, 0);
        float totalWidth = cols + cols * spacing.x -spacing.x;
        Vector3 startingPosition = origin.position;
        startingPosition.x = -totalWidth / 2f;
        

        for (int i = 0; i < _cards.Count; i++)
        {
            if(constraint == Constrain.FixedColumCount)
            {
                startingPosition.y = Mathf.Floor(i / maxCol) * -1.6f;
            }
            _cards[i].transform.position = startingPosition + Vector3.right * (i % maxCol) * (spacing.x + cardWidth) + pivotOffset;
        }
    }
}
