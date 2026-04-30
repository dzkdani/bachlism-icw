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

    private StatSystem statSystem;

    private void Start()
    {
        if (GameController.Instance != null)
        {
            statSystem = GameController.Instance.StatSystem;
            if (statSystem != null)
            {
                statSystem.OnStatsChanged += UpdateStatsDisplay;
                UpdateStatsDisplay();
            }
            else
            {
                Debug.LogError("StatSystem instance is null in UIManager.");
            }
        }
        else
        {
            Debug.LogError("GameController not found!");
        }
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


    private void OnDestroy()
    {
        if (statSystem != null)
            statSystem.OnStatsChanged -= UpdateStatsDisplay;
    }
}
