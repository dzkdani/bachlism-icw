using UnityEngine;
using System;

public class StatSystem
{
    public float Environment = 0f;
    public float Economy = 0f;
    public float Trust = 0f;
    public float Corruption = 0f;
    public int Turn = 0;

    public event Action OnStatsChanged;

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
        if (Corruption >= 100) return true;
        if (Trust <= 0) return true;
        if (Environment <= 0) return true;
        if (Economy <= 0) return true;
        else return false;
    }

    public bool IsWin()
    {
        if (Turn >= GameController.Instance.TargetTurn) return true;
        else return false;
    }
}
