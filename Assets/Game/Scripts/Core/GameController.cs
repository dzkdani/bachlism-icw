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
        currentState = GameState.Start;
        stat = new StatSystem(); // Reset stats for new game
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

        stat.OnStatsChanged += OnCheckGameOver; // Subscribe to stat changes to check for game over conditions
    }

    public void OnDrawCard()
    {
        currentState = GameState.DrawCard;
        // Logic to draw a card and present it to the player would go here
    }

    public void OnCheckGameOver()
    {
        currentState = GameState.CheckGameOver;
        if (stat.IsLose())
        {
            Debug.Log("Game Over!");
            currentState = GameState.End;
        }
        else if (stat.IsWin())
        {
            Debug.Log("You Win!");
            currentState = GameState.End;
        }
        else
        {
            // If not game over, start next turn
            OnDrawCard();
        }
    }
}
