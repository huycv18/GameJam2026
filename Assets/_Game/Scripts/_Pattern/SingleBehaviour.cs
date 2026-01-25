using System;
using System.Collections.Generic;
using UnityEngine;

public static class SingleBehaviour
{
    private static readonly Dictionary<Type, MonoBehaviour> _instances = new();

    public static T Of<T>() where T : MonoBehaviour
    {
        var type = typeof(T);

        if (_instances.TryGetValue(type, out var instance))
            return (T)instance;

        // tìm trong scene trước
        var existing = UnityEngine.Object.FindObjectOfType<T>();
        if (existing != null)
        {
            _instances[type] = existing;
            return existing;
        }

        // không có thì tạo mới
        var go = new GameObject(type.Name);
        var created = go.AddComponent<T>();
        UnityEngine.Object.DontDestroyOnLoad(go);

        _instances[type] = created;
        return created;
    }

    public static bool Has<T>() where T : MonoBehaviour
    {
        return _instances.ContainsKey(typeof(T));
    }

    public static void Clear<T>() where T : MonoBehaviour
    {
        if (_instances.TryGetValue(typeof(T), out var instance))
        {
            UnityEngine.Object.Destroy(instance.gameObject);
            _instances.Remove(typeof(T));
        }
    }

    public static void ClearAll()
    {
        foreach (var inst in _instances.Values)
        {
            if (inst != null)
                UnityEngine.Object.Destroy(inst.gameObject);
        }
        _instances.Clear();
    }
}
