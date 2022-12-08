using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandeable
{
    void AddToHand(Card card);
    void RemoveFromHand(Card card);
    void ReorderCards();
}
