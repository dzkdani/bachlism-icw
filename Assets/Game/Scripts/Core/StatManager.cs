using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    environment,
    economy,
    trust,
    corruption
}

public class StatManager : MonoBehaviour
{
    private Dictionary<StatType, float> stats = new Dictionary<StatType, float>();

    public void ModifyStat(StatType type, float amount)
    {
        stats[type] += amount;
        stats[type] = Mathf.Clamp(stats[type], 0, 100); 

        EventBus.OnStatUpdated?.Invoke(type, stats[type]);
    }
}
