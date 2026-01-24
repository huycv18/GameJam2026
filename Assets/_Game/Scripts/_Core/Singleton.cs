using System;
using System.Collections.Generic;

public static class Singleton
{
    private static readonly Dictionary<Type, object> _instances = new();

    public static T Of<T>() where T : class, new()
    {
        var type = typeof(T);

        if (!_instances.TryGetValue(type, out var instance))
        {
            instance = new T();
            _instances[type] = instance;
        }

        return (T)instance;
    }

    public static bool Has<T>()
    {
        return _instances.ContainsKey(typeof(T));
    }

    public static void Clear<T>()
    {
        _instances.Remove(typeof(T));
    }

    public static void ClearAll()
    {
        _instances.Clear();
    }
}
