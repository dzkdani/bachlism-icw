using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _events = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        var type = typeof(T);

        if (_events.TryGetValue(type, out var existing))
            _events[type] = Delegate.Combine(existing, listener);
        else
            _events[type] = listener;
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        var type = typeof(T);

        if (!_events.TryGetValue(type, out var existing)) return;

        var current = Delegate.Remove(existing, listener);

        if (current == null)
            _events.Remove(type);
        else
            _events[type] = current;
    }

    public static void Publish<T>(T eventData)
    {
        if (_events.TryGetValue(typeof(T), out var del))
        {
            if (del is Action<T> callback)
                callback.Invoke(eventData);
        }
    }

    // Extension-style usage inside same class
    public static void PublishEvent<T>(this T eventData)
    {
        Publish(eventData);
    }

    public static void SubscribeEvent<T>(this Action<T> listener)
    {
        Subscribe(listener);
    }

    public static void UnsubscribeEvent<T>(this Action<T> listener)
    {
        Unsubscribe(listener);
    }
}