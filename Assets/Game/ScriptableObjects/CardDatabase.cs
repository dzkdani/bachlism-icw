using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardDatabase", order = 1)]
public class CardDatabase : ScriptableObject 
{   
    public List<CardData> cards;

    public CardData GetRandomCard()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogError("CardDatabase is empty! Cannot retrieve a card.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        return cards[randomIndex];
    }

}