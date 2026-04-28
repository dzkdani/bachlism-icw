using UnityEngine;
using System;
using System.Collections;

public enum GameState
{
    Start,
    DrawCard,
    AwaitDecision,
    ApplyResult,
    CheckGameOver,
    End
}

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] private int targetTurn = 100;
    public int TargetTurn => targetTurn;

    [SerializeField] private CardManager cardManager;
    [SerializeField] private float delayBeforeDeploy = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private GameState currentState;
    private StatSystem stat;
    public GameState CurrentState => currentState;
    public StatSystem StatSystem => stat;

    public event Action OnDrawCardRequested;
    public event Action OnGameEnded;

    void Start()
    {
        if (cardManager == null)
            cardManager = FindFirstObjectByType<CardManager>();

        cardManager.OnAllCardsDeployed -= OnStartGame;
        cardManager.OnAllCardsDeployed += OnStartGame;

        stat = new StatSystem();
        stat.OnStatsChanged += OnResolve;

        if (cardManager != null)
        {
            StartCoroutine(InitializeGame());
        }
        else
        {
            Debug.LogError("CardManager not found! Cannot initialize game.");
        }
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(delayBeforeDeploy);
        cardManager.DeployCards();
    }

    // void Update()
    // {
    //     switch (currentState)
    //     {
    //         case GameState.Start:
    //             // Handle start logic if needed
    //             break;
    //         case GameState.DrawCard:
    //             // Handle card drawing logic
    //             break;
    //         case GameState.AwaitDecision:
    //             // Waiting for player input, no update logic needed here
    //             break;
    //         case GameState.ApplyResult:
    //             // Results are applied immediately in OnApplyResult, so we can transition to the next state
    //             break;
    //         case GameState.CheckGameOver: 
    //             // Check for game over or win conditions
    //             break;
    //         case GameState.End:
    //             // Handle end game logic if needed
    //             break;
    //     }
    // }

    public void OnNewGame()
    {
        stat.OnStatsChanged -= OnResolve;
        stat = new StatSystem();
        stat.OnStatsChanged += OnResolve;

        currentState = GameState.Start;
        
        if (cardManager != null)
        {
            StartCoroutine(InitializeGame());
        }
    }

    public void OnStartGame()
    {
        currentState = GameState.Start;
        OnDrawCard();
    }

    public void OnAwaitDecision()
    {
        currentState = GameState.AwaitDecision;
    }

    public void OnApplyResult(Decision choice)
    {
        currentState = GameState.ApplyResult;
        stat.ApplyEffect(choice.effects);
        stat.AddTurn();
    }

    public void OnDrawCard()
    {
        currentState = GameState.DrawCard;
        OnDrawCardRequested?.Invoke();
        OnAwaitDecision();
    }

    public void OnResolve()
    {
        currentState = GameState.CheckGameOver;

        if (stat.IsLose())
        {
            Debug.Log("Game Over!");
            currentState = GameState.End;
            OnGameEnded?.Invoke();
        }
        else if (stat.IsWin())
        {
            Debug.Log("You Survived 100 Turns! You Win!");
            currentState = GameState.End;
            OnGameEnded?.Invoke();
        }
        else
        {
            OnDrawCard();
        }
    }
}
