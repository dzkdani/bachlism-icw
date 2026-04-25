using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;
using NaughtyAttributes;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckTarget;
    [SerializeField] private float cardAnimationDuration = 0.5f;
    [SerializeField] private float delayBetweenCards = 0.1f;
    [SerializeField] private int deployedCardCount = 17;

    private List<GameObject> deployedCards = new List<GameObject>();
    public CardDatabase cardDatabase;
    public event Action OnAllCardsDeployedEvent;

    void OnEnable()
    {
        InputManager.Instance.OnActiveDropped += OnDecisionMade;
    }

    void OnDisable()
    {
        InputManager.Instance.OnActiveDropped -= OnDecisionMade;
    }

    [Button("Deploy Cards")]
    public void DeployCards()
    {
        deployedCards.Clear();
        
        // Spawn and animate cards
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
                    for (int j = 0; j < deployedCards.Count - 2; j++)
                    {
                        ObjectPool.Return(deployedCards[j]);
                    }
                    
                    // Invoke event for all listeners that cards are deployed and ready
                    OnAllCardsDeployedEvent?.Invoke();
                });
            }
        }
    }

    public void OnDecisionMade(InputHandler dropObj, InputManager.DropZone dropZone)
    {
        if (dropZone == InputManager.DropZone.None)
            return;

        CardInfo card = dropObj.GetComponent<CardInfo>();
        if (card == null)
            return;
        
        if (dropZone == InputManager.DropZone.Left)
            GameController.Instance.OnApplyResult(card.Left);
        else if (dropZone == InputManager.DropZone.Right)
            GameController.Instance.OnApplyResult(card.Right);

        GenerateNextCard();
    }

    private void GenerateNextCard()
    {
        GameObject cardInstance = ObjectPool.Get(cardPrefab, deckTarget);
        cardInstance.transform.localScale = Vector3.one;
        cardInstance.transform.position = deckTarget.position;
    }
}
