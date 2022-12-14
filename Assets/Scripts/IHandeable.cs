using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandeable
{
    void AddToHand(Card card, bool isNew = false);
    void RemoveFromHand(Card card);
    void ReorderActualCards(int newCardsToAdd);
}
