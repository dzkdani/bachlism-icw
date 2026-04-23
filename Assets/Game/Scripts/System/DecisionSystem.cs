using UnityEngine;

public static class DecisionSystem
{
    public static void Resolve(this Decision choice, StatSystem statSystem = null)
    {
        if (statSystem == null) return;
        
        statSystem.ApplyEffect(choice.effects);
    }

    public static void Hold(this Decision choice)
    {
        // Optional: maybe log, delay, or queue decision
        Debug.Log("Decision held");
    }
}