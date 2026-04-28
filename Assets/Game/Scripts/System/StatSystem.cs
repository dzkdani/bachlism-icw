using UnityEngine;
using System;

public class StatSystem
{
    public float Environment { get; private set; }
    public float Economy { get; private set; }
    public float Trust { get; private set; }
    public float Corruption { get; private set; }
    public int Turn { get; private set; }

    public event Action OnStatsChanged;

    public StatSystem()
    {
        // Initialize with random values between 10-50 as per design
        Environment = UnityEngine.Random.Range(10f, 51f);
        Economy = UnityEngine.Random.Range(10f, 51f);
        Trust = UnityEngine.Random.Range(10f, 51f);
        Corruption = UnityEngine.Random.Range(10f, 51f);
        Turn = 0;
    }

    public void AddTurn()
    {
        Turn++;
    }

    public void ApplyEffect(Effect[] effects)
    {
        foreach (Effect changes in effects)
        {
            ModifyStat(changes.stat, changes.amount);
        }
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

        OnStatsChanged?.Invoke();
    }

    public bool IsLose()
    {
        // if (Corruption >= 100) return true;
        // if (Trust <= 0) return true;
        // if (Environment <= 0) return true;
        // if (Economy <= 0) return true;
        // else 
            return false;
    }

    public bool IsWin()
    {
        if (Turn >= GameController.Instance.TargetTurn) return true;
        else return false;
    }
}
