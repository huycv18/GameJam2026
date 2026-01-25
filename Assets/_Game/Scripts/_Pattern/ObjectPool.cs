using System.Collections.Generic;
using UnityEngine;
public class ObjectPool
{
    private readonly Queue<GameObject> pool = new();
    private readonly GameObject prefab;

    public ObjectPool(GameObject prefab, int size)
    {
        this.prefab = prefab;

        for (int i = 0; i < size; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(this.prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : UnityEngine.Object.Instantiate(this.prefab);
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

}
