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
    [SerializeField] private GoogleSheetLoader sheetLoader;
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
            stat = new StatSystem();
        }
    }

    void Start()
    {
        if (cardManager == null)
            cardManager = FindFirstObjectByType<CardManager>();

        if (cardManager != null)
        {
            cardManager.OnAllCardsDeployed -= OnStartGame;
            cardManager.OnAllCardsDeployed += OnStartGame;
        }

        stat.OnStatsChanged += OnResolve;

        if (stat.HasSave()) 
            stat.Load(); 
    }

    [SerializeField] private GameState currentState;
    private StatSystem stat;
    public GameState CurrentState => currentState;
    public StatSystem StatSystem => stat;

    public event Action OnDrawCardRequested;
    public event Action OnGameEnded;

    public void BeginGame()
    {
        if (cardManager == null)
            cardManager = FindFirstObjectByType<CardManager>();

        if (sheetLoader == null)
            sheetLoader = FindFirstObjectByType<GoogleSheetLoader>();

        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // load cards from online sheet
        if (sheetLoader != null)
        {
            yield return sheetLoader.LoadCards();

            if (sheetLoader.RuntimeDatabase != null)
            {
                cardManager.SetCards(sheetLoader.RuntimeDatabase);
            }
            else
            {
                Debug.LogWarning("No online cards loaded. Using fallback.");
            }
        }

        yield return new WaitForSeconds(delayBeforeDeploy);

        cardManager.DeployCards();
        
        AudioManager.Instance.Play("BGM");
    }

    public void OnNewGame()
    {
        stat.OnStatsChanged -= OnResolve;
        stat = new StatSystem();
        stat.OnStatsChanged += OnResolve;

        currentState = GameState.Start;
        
        cardManager.DeployCards();
    }

    public void OnStartGame()
    {
        currentState = GameState.AwaitDecision;
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
