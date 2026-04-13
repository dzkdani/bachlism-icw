using UnityEngine;

public class DecisionSystem
{
    public void ResolveDecision(Choice choice)
    {
        foreach (var effect in choice.effect)
        {
            EventBus.OnStatChanged?.Invoke(effect);
        }
    }
}
