using System;

/// <summary>
/// Data structure representing the game state.
/// Designed to be serializable for local storage or future cloud synchronization.
/// </summary>
[Serializable]
public class GameData
{
    public float Environment;
    public float Economy;
    public float Trust;
    public float Corruption;
    public int Turn;
    
    // Seed allows reproducing the exact RNG state for consistent gameplay upon loading
    public int Seed;
}