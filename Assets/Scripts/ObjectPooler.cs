csharp
using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance; // Static instance for easy access

    [SerializeField]
    private GameObject prefabToPool; // The prefab to pool
    [SerializeField]
    private int poolSize = 10; // The initial size of the pool

    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this; // Set the static instance

        pooledObjects = new List<GameObject>();

        // Instantiate the initial pool of objects
        for (int i = 0; i < poolSize; i++)
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
                // Potential: Call a method on the object to reset its state if needed
                // obj.GetComponent<IPooledObject>()?.OnObjectSpawn();
                return obj;
            }
        }

        // If no inactive object is found, optionally grow the pool (be cautious with performance)
        // GameObject newObj = Instantiate(prefabToPool);
        // newObj.SetActive(true);
        // newObj.transform.position = position;
        // newObj.transform.rotation = rotation;
        // pooledObjects.Add(newObj);
        // Potential: newObj.GetComponent<IPooledObject>()?.OnObjectSpawn();
        // Debug.LogWarning("Object pool is depleted. Consider increasing the pool size.");
        // return newObj;

        // Or return null if the pool is depleted and cannot grow
        Debug.LogWarning("Object pool is depleted. Returning null.");
        return null;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        // Deactivate the object and return it to the pool (logically)
        objectToReturn.SetActive(false);
        // Potential: Reset the object's state before returning if needed
        // objectToReturn.GetComponent<IPooledObject>()?.OnObjectReturn();
    }
}