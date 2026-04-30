using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public CardDatabase cardDatabase;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckTarget;
    [SerializeField] private float cardAnimationDuration = 0.5f;
    [SerializeField] private float delayBetweenCards = 0.25f;
    [SerializeField] private int deployedCardCount = 12;

    [SerializeField] private GameObject currentCard;
    [SerializeField] private GameObject nextCard;
    [SerializeField] private bool isResolving;

    public event Action OnAllCardsDeployed;

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnActiveDropped += OnDecisionMade;
        else
            Debug.LogWarning("InputManager instance not found. Decision making will not work.");

        if (GameController.Instance != null)
            GameController.Instance.OnDrawCardRequested += TransitionToNextCard;
        else
            Debug.LogWarning("GameController instance not found. Card transitions will not work.");
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnActiveDropped -= OnDecisionMade;
        else
            Debug.LogWarning("InputManager instance not found. Could not unbind decision making.");

        if (GameController.Instance != null)
            GameController.Instance.OnDrawCardRequested -= TransitionToNextCard;
        else
            Debug.LogWarning("GameController instance not found. Could not unbind card transitions.");
    }

    public void SetCards(CardDatabase database)
    {
        cardDatabase = database;
    }

    // [Button("Deploy Cards")]
    public void DeployCards()
    {
        // Pre-populate the object pool with cards
        List<GameObject> tempCards = new List<GameObject>();
        for (int i = 0; i < deployedCardCount; i++)
        {
            GameObject cardInstance = ObjectPool.Get(cardPrefab, deckTarget);
            tempCards.Add(cardInstance);

            // Position card above the canvas (high Z or Y position)
            Vector3 spawnPos = deckTarget.position;
            spawnPos.y += 5000f; // Spawn above canvas
            cardInstance.transform.position = spawnPos;

            // Animate to deck with staggered delay
            float delayTime = i * delayBetweenCards;
            Sequence cardSequence = DOTween.Sequence();
            
            cardSequence.AppendInterval(delayTime);
            cardSequence.AppendCallback(() => AudioManager.Instance.Play("draw"));
            cardSequence.Append(cardInstance.transform.DOMove(deckTarget.position, cardAnimationDuration)
                .SetEase(Ease.InOutQuad));

            // On last card, initialize the card pipeline
            if (i == deployedCardCount - 1)
            {
                cardSequence.OnComplete(() =>
                {
                    currentCard = tempCards[0];
                    // Return all temp cards to pool except the first
                    for (int j = 1; j < tempCards.Count; j++)
                    {
                        ObjectPool.Return(tempCards[j]);
                    }

                    InitializeCard(currentCard);
                    PrepareNextCard();
                    ShowCurrentCard();
                    OnAllCardsDeployed?.Invoke();
                });
            }
        }
    }

    private void OnDecisionMade(InputHandler dropObj, InputManager.DropZone dropZone)
    {
        if (isResolving)
            return;
        if (dropZone == InputManager.DropZone.None)
            return;

        CardInfo card = dropObj.GetComponent<CardInfo>();
        if (card == null)
            return;

        // Block further input via CanvasGroup instead of isResolving
        CanvasGroup cg = currentCard.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        
        if (dropZone == InputManager.DropZone.Left)
            GameController.Instance.OnApplyResult(card.Left);
        else if (dropZone == InputManager.DropZone.Right)
            GameController.Instance.OnApplyResult(card.Right);
    }

    /// <summary>
    /// Prepares the next card by fetching it from the pool and initializing it with random card data.
    /// This maintains the 2-active-cards pipeline by having the next card ready before transitions.
    /// Only prepares a new card if nextCard is null.
    /// </summary>
    private void PrepareNextCard()
    {
        // Don't prepare if nextCard is already prepared
        if (nextCard != null)
            return;

        // Get a card from the pool
        nextCard = ObjectPool.Get(cardPrefab, deckTarget);
        nextCard.transform.localScale = Vector3.one;
        nextCard.transform.position = deckTarget.position;
        CanvasGroup cg = nextCard.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        nextCard.transform.SetAsFirstSibling(); // Ensure it's behind the current card

        InitializeCard(nextCard);
    }

    /// <summary>
    /// Transitions from the current card to the next card in the pipeline.
    /// Fades out the current card and returns it to the pool, then shows the prepared next card.
    /// </summary>
    private void TransitionToNextCard()
    {
        if (isResolving)
            return;

        isResolving = true;
        StartCoroutine(CardTransition());
    }

    private IEnumerator CardTransition()
    {
        if (nextCard == null)
        {
            isResolving = false;
            yield break;
        }

        GameObject oldCurrent = currentCard;
        InputHandler oldInput = oldCurrent.GetComponent<InputHandler>();

        currentCard = nextCard;
        nextCard = null;

        yield return oldInput.ChooseSequence().WaitForCompletion();

        ShowCurrentCard();

        yield return null;

        ObjectPool.Return(oldCurrent);

        PrepareNextCard();
        
        isResolving = false;
    }

    /// <summary>
    /// Shows the current card.
    /// </summary>
    private void ShowCurrentCard()
    {
        if (currentCard != null)
        {
            currentCard.SetActive(true);
            CanvasGroup cg = currentCard.GetComponent<CanvasGroup>();
            cg.interactable = true;
            cg.blocksRaycasts = true;
            currentCard.transform.SetAsLastSibling(); 
        }
    }

    private void InitializeCard(GameObject card)
    {
        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase is null. Cannot initialize card.");
            return;
        }
            
        CardInfo cardInfo = card.GetComponent<CardInfo>();
        CardData randomCard = cardDatabase.GetRandomCard();
        cardInfo.Initialize(randomCard);
    }
}
