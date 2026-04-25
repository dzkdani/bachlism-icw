using UnityEngine;

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
    private int currentTurn = 0;
    [SerializeField] private int targetTurn = 100;
    public int TargetTurn => targetTurn;

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
            DontDestroyOnLoad(gameObject); 
        }
    }

    [SerializeField] private GameState currentState;
    private StatSystem stat;
    public GameState CurrentState => currentState;


    void Start()
    {
        stat.OnStatsChanged += EvaluateGameState; 

        stat = new StatSystem();
        OnStartGame();       
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                // Handle start logic if needed
                break;
            case GameState.DrawCard:
                // Handle card drawing logic
                break;
            case GameState.AwaitDecision:
                // Waiting for player input, no update logic needed here
                break;
            case GameState.ApplyResult:
                // Results are applied immediately in OnApplyResult, so we can transition to the next state
                break;
            case GameState.CheckGameOver: 
                // Check for game over or win conditions
                break;
            case GameState.End:
                // Handle end game logic if needed
                break;
        }
    }

    public void OnNewGame()
    {
        stat.OnStatsChanged -= EvaluateGameState;
        stat = new StatSystem();
        stat.OnStatsChanged += EvaluateGameState;

        currentTurn = 0;
        currentState = GameState.Start;
    }

    public void OnStartGame()
    {
        currentState = GameState.Start;        
    }

    public void OnAwaitDecision()
    {
        currentState = GameState.AwaitDecision;
    }

    public void OnApplyResult(Decision choice)
    {
        currentState = GameState.ApplyResult;

        choice.Resolve(stat);
        currentTurn++;

        EvaluateGameState();
    }

    public void OnDrawCard()
    {
        currentState = GameState.DrawCard;
        // Logic to draw a card and present it to the player would go here
    }

    public void EvaluateGameState()
    {
        currentState = GameState.CheckGameOver;

        if (stat.IsLose())
        {
            Debug.Log("Game Over!");
            currentState = GameState.End;
        }
        else if (currentTurn >= targetTurn)
        {
            Debug.Log("You Survived 100 Turns! You Win!");
            currentState = GameState.End;
        }
        else
        {
            OnDrawCard();
        }
    }
}
