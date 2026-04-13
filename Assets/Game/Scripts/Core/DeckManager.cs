using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;
using NaughtyAttributes;
using UnityEngine.Events;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckTarget; // The deck center location
    [SerializeField] private float cardAnimationDuration = 0.5f;
    [SerializeField] private float delayBetweenCards = 0.1f;
    [SerializeField] private int deployedCardCount = 17;
    
    public List<CardData> deck;
    
    private List<GameObject> deployedCards = new List<GameObject>();

    /// <summary>
    /// Event invoked when all cards have been deployed to the deck.
    /// </summary>
    public event Action OnAllCardsDeployedEvent;
    public UnityEvent OnAllCardsDeployedUnityEvent; 
    void Start()
    {
        OnAllCardsDeployedEvent += () => Debug.Log("All cards deployed to deck!"); // Example listener
        OnAllCardsDeployedUnityEvent.AddListener(() => Debug.Log("All cards deployed to deck!")); // Example listener
    }

    [Button("Deploy Cards To Deck")]
    public void DeployCardsToDeck()
    {
        deployedCards.Clear();
        
        // Spawn and animate 17 cards
        for (int i = 0; i < deployedCardCount; i++)
        {
            GameObject cardInstance = ObjectPool.Get(cardPrefab, deckTarget);
            deployedCards.Add(cardInstance);

            // Position card above the canvas (high Z or Y position)
            Vector3 spawnPos = deckTarget.position;
            spawnPos.y += 2000f; // Spawn above canvas
            cardInstance.transform.position = spawnPos;

            // Animate to deck with staggered delay
            float delayTime = i * delayBetweenCards;
            Sequence cardSequence = DOTween.Sequence();
            
            cardSequence.AppendInterval(delayTime);
            cardSequence.Append(cardInstance.transform.DOMove(deckTarget.position, cardAnimationDuration)
                .SetEase(Ease.InOutQuad));

            // On last card, add callback and return others to pool
            if (i == deployedCardCount - 1)
            {
                cardSequence.OnComplete(() =>
                {
                    // Return all cards to pool except the last one
                    for (int j = 0; j < deployedCards.Count - 1; j++)
                    {
                        ObjectPool.Return(deployedCards[j]);
                    }
                    
                    // Invoke event for all listeners
                    OnAllCardsDeployedEvent?.Invoke();
                    OnAllCardsDeployedUnityEvent?.Invoke();
                });
            }
        }
    }

    public CardData DrawNextCard()
    {
        return deck[UnityEngine.Random.Range(0, deck.Count)]; // Placeholder for drawing a random card from the deck
    }
}
