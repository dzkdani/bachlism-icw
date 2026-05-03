using UnityEngine;
using System;
using System.IO;

public class StatSystem
{
    public float Environment { get; private set; }
    public float Economy { get; private set; }
    public float Trust { get; private set; }
    public float Corruption { get; private set; }
    public int Turn { get; private set; }

    public event Action OnStatsChanged;

    // Path for local save. Easily swappable for cloud sync by returning JSON string instead of file IO.
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");


    public StatSystem()
    {
        // Initialize with random values between 20-50 as per design
        Environment = UnityEngine.Random.Range(20f, 51f);
        Economy = UnityEngine.Random.Range(20f, 51f);
        Trust = 49f;
        Corruption = UnityEngine.Random.Range(20f, 51f);
        Turn = 1;
    }

    public void AddTurn()
    {
        Turn++;
    }

    public void ApplyEffect(Effect[] effects)
    {
        if (effects != null && effects.Length > 0)
        {
            foreach (Effect changes in effects)
            {
                Debug.Log($"Applying effect: {changes.stat} change by {changes.amount}");
                ModifyStat(changes.stat, changes.amount);
            }
        }

        // Always invoke after all effects are applied
        OnStatsChanged?.Invoke();
    }

    private void ModifyStat(Stat type, float amount)
    {
        switch (type)
        {
            case Stat.environment:
                Environment = Mathf.Clamp(Environment + amount, 0f, 100f);
                break;
            case Stat.economy:
                Economy = Mathf.Clamp(Economy + amount, 0f, 100f);
                break;
            case Stat.trust:
                Trust = Mathf.Clamp(Trust + amount, 0f, 100f);
                break;
            case Stat.corruption:
                Corruption = Mathf.Clamp(Corruption + amount, 0f, 100f);
                break;
        }
    }

    public bool IsLose()
    {
        if (Corruption >= 100) return true;
        if (Trust <= 0) return true;
        if (Environment <= 0) return true;
        if (Economy <= 0) return true;
        else 
            return false;
    }

    public bool IsWin()
    {
        if (Turn >= GameController.Instance.TargetTurn) return true;
        else return false;
    }

     /// <summary>
    /// Checks if a local save file exists.
    /// </summary>
    public bool HasSave() => File.Exists(SavePath);

    /// <summary>
    /// Saves the current state to a local JSON file.
    /// Scalable for cloud: simply return the JSON string and upload instead of writing to disk.
    /// </summary>
    public void Save()
    {
        var data = ToGameData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved successfully.");
    }

    /// <summary>
    /// Loads the state from the local JSON file if it exists.
    /// </summary>
    public void Load()
    {
        if (!HasSave()) return;

        string json = File.ReadAllText(SavePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        LoadFromData(data);
        Debug.Log("Game loaded successfully.");
    }

    /// <summary>
    /// Serializes current stats and RNG seed into a GameData object.
    /// </summary>
    public GameData ToGameData()
    {
        return new GameData
        {
            Environment = this.Environment,
            Economy = this.Economy,
            Trust = this.Trust,
            Corruption = this.Corruption,
            Turn = this.Turn,
            Seed = UnityEngine.Random.seed
        };
    }

    /// <summary>
    /// Loads stats from a GameData object.
    /// </summary>
    public void LoadFromData(GameData data)
    {
        this.Environment = data.Environment;
        this.Economy = data.Economy;
        this.Trust = data.Trust;
        this.Corruption = data.Corruption;
        this.Turn = data.Turn;
    
        // Trigger UI update and checks
        OnStatsChanged?.Invoke();
    }
}

    [System.Serializable]
    public class GameData
    {
        public float Environment;
        public float Economy;
        public float Trust;
        public float Corruption;
        public int Turn;
        public int Seed;
    }
