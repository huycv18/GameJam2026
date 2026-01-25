using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolItem
{
    public GameObject prefab;
    public int initialSize;
}

public sealed class PoolController : MonoBehaviour
{
    private Dictionary<Type, ObjectPool> _pools = new();

    [SerializeField] private List<PoolItem> poolItems = new();


    // Start is called before the first frame update
    void Awake()
    {
        foreach (var item in poolItems)
        {
            var pool = new ObjectPool(item.prefab, item.initialSize);
            _pools.Add(item.prefab.GetType(), pool);
        }

    }

    public void CreatePool(GameObject prefab, int initialSize = 2)
    {
        _pools.Add(prefab.GetType(), new ObjectPool(prefab, initialSize));
    }

    public ObjectPool GetPool(GameObject prefab)
    {
        if (_pools.TryGetValue(prefab.GetType(), out ObjectPool pool))
        {
            return pool;
        }
        
        this.CreatePool(prefab, 2);
        return _pools[prefab.GetType()];
    }
}
