using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI display for game stats and end-game screen.
/// Integrates with StatBar components for animated stat visualization.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private StatBar environmentBar;
    [SerializeField] private StatBar economyBar;
    [SerializeField] private StatBar trustBar;
    [SerializeField] private StatBar corruptionBar;

    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Button restartButton;

    private StatSystem statSystem;

    private void Start()
    {
        if (GameController.Instance != null)
        {
            statSystem = GameController.Instance.StatSystem;
            if (statSystem != null)
            {
                statSystem.OnStatsChanged += UpdateStatsDisplay;
                UpdateStatsDisplay();  // Initial display
            }

            GameController.Instance.OnGameEnded += ShowEndGameScreen;
        }
        else
        {
            Debug.LogError("GameController not found!");
        }

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    private void UpdateStatsDisplay()
    {
        if (statSystem == null) return;

        // Update stat bars with animation
        if (environmentBar != null)
            environmentBar.UpdateStat(statSystem.Environment);
        if (economyBar != null)
            economyBar.UpdateStat(statSystem.Economy);
        if (trustBar != null)
            trustBar.UpdateStat(statSystem.Trust);
        if (corruptionBar != null)
            corruptionBar.UpdateStat(statSystem.Corruption);

        // Update turn counter
        if (turnText != null)
            turnText.text = $"Day: {GameController.Instance.StatSystem.Turn}";
    }

    private void ShowEndGameScreen()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        GameController.Instance.OnNewGame();
    }

    private void OnDestroy()
    {
        if (statSystem != null)
            statSystem.OnStatsChanged -= UpdateStatsDisplay;

        if (GameController.Instance != null)
            GameController.Instance.OnGameEnded -= ShowEndGameScreen;

        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);
    }
}
