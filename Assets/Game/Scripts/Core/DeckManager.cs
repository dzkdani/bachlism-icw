using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<CardData> deck;

    public CardData DrawNextCard()
    {
        return deck[Random.Range(0, deck.Count)]; // Placeholder for drawing a random card from the deck
    }
}
