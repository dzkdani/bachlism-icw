using System;

public static class EventBus
{
    public static Action<Choice> OnChoiceMade;
    public static Action<Effect> OnStatChanged;
    public static Action<StatType, float> OnStatUpdated;
}