csharp
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance; // Static instance for easy access

    [SerializeField]
    private GameObject prefabToPool; // The prefab to pool
    [SerializeField]
 private int initialPoolSize = 10; // The initial size of the pool

    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this; // Set the static instance

        pooledObjects = new List<GameObject>();

        // Instantiate the initial pool of objects
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.SetActive(false); // Deactivate them
            pooledObjects.Add(obj);
        }
    }

    public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
    {
        // Find an inactive object in the pool
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                // Found an inactive object, configure and return it
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, optionally grow the pool (be cautious with performance)
        Debug.LogWarning("Pool is empty. Expanding pool for: " + prefabToPool.name);
 GameObject newObj = Instantiate(prefabToPool);
 newObj.SetActive(true); // Activate the new object immediately
 pooledObjects.Add(newObj); // Add it to the pool
 newObj.transform.position = position;
 newObj.transform.rotation = rotation;
 return newObj;
        // Potential: newObj.GetComponent<IPooledObject>()?.OnObjectSpawn();
        // Debug.LogWarning("Object pool is depleted. Consider increasing the pool size.");
        // return newObj;

        // Or return null if the pool is depleted and cannot grow
        Debug.LogWarning("Object pool is depleted. Returning null.");
        return null;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        // You might want to reset its state here (e.g., health, velocity, animation)
    }
}