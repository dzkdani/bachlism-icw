using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple static object pool helper for reusable pooling of any GameObject.
/// </summary>
public static class ObjectPool
{
    private static Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// Get or create a pool for a given prefab and get an instance from it.
    /// </summary>
    public static GameObject Get(GameObject prefab, Transform parent = null)
    {
        string prefabName = prefab.name;
        
        // Create pool if it doesn't exist
        if (!pools.ContainsKey(prefabName))
        {
            pools[prefabName] = new Queue<GameObject>();
        }

        GameObject instance;
        
        // Get from pool or instantiate new
        if (pools[prefabName].Count > 0)
        {
            instance = pools[prefabName].Dequeue();
            instance.SetActive(true);
        }
        else
        {
            instance = Object.Instantiate(prefab, parent);
            instance.name = prefabName;
        }

        // Set parent if provided
        if (parent != null && instance.transform.parent != parent)
        {
            instance.transform.SetParent(parent);
        }

        return instance;
    }

    /// <summary>
    /// Return an instance back to the pool.
    /// </summary>
    public static void Return(GameObject instance)
    {
        if (instance == null) return;

        string prefabName = instance.name;
        
        if (!pools.ContainsKey(prefabName))
        {
            pools[prefabName] = new Queue<GameObject>();
        }

        instance.SetActive(false);
        pools[prefabName].Enqueue(instance);
    }

    /// <summary>
    /// Clear a specific pool.
    /// </summary>
    public static void ClearPool(string prefabName)
    {
        if (pools.ContainsKey(prefabName))
        {
            while (pools[prefabName].Count > 0)
            {
                GameObject obj = pools[prefabName].Dequeue();
                Object.Destroy(obj);
            }
            pools.Remove(prefabName);
        }
    }

    /// <summary>
    /// Clear all pools.
    /// </summary>
    public static void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Object.Destroy(obj);
            }
        }
        pools.Clear();
    }
}
