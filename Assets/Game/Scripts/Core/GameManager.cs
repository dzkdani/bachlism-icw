using UnityEngine;

public enum GameState
{
    decision,
    resolution
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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

    public DeckManager Deck { get; private set; }
    public StatManager Stats { get; private set; }

    void Start()
    {
        
    }
}
