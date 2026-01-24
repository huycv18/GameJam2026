using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Enum, List<Delegate>> listeners = new();

    public static void On(Enum eventType, Delegate callback)
    {
        if (!listeners.ContainsKey(eventType))
            listeners[eventType] = new();

        listeners[eventType].Add(callback);
    }

    public static void Off(Enum eventType, Delegate callback)
    {
        if (!listeners.ContainsKey(eventType))
            return;

        listeners[eventType].Remove(callback);

        if (listeners[eventType].Count == 0)
            listeners.Remove(eventType);
    }

    public static void Emit(Enum eventType, params object[] args)
    {
        if (!listeners.TryGetValue(eventType, out var delegates))
            return;

        foreach (var callback in delegates)
        {
            var parameters = callback.Method.GetParameters();
            if (parameters.Length != args.Length)
            {
                Console.WriteLine(
                    $"⚠️ Event '{eventType}' arg mismatch ({parameters.Length} expected, got {args.Length})"
                );
                continue;
            }

            try
            {
                callback.DynamicInvoke(args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"⚠️ Event '{eventType}' invoke failed: {e.Message}");
            }
        }
    }
}
