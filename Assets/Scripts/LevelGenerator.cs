csharp
using UnityEngine;
using System.Collections.Generic; // Required for List

public class LevelGenerator : MonoBehaviour // Corrected class definition
{
    [SerializeField]
    private GameObject[] levelChunkPrefabs; // Array of level chunk prefabs
    [SerializeField]
    private float chunkLength = 20f; // The length of each level chunk (adjust based on your prefabs)
    [SerializeField]
    private int chunksToGenerateAhead = 3; // How many chunks to keep generated ahead of the player
    [SerializeField]
    private Transform playerTransform; // Assign the player's transform to track their position
    [SerializeField]
    private EnemySpawner enemySpawner; // Assign the EnemySpawner in the Inspector
    [SerializeField]
    private float destroyChunkDistance = 10f; // Distance behind the player at which to destroy old chunks

    private Vector3 lastChunkEndPosition;
    private List<GameObject> activeChunks = new List<GameObject>(); // List to keep track of active chunks

    void Start()
    {
        if (levelChunkPrefabs == null || levelChunkPrefabs.Length == 0)
        {
            Debug.LogError("No level chunk prefabs assigned to the LevelGenerator!");
            enabled = false; // Disable the script if no prefabs
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to the LevelGenerator!");
            enabled = false; // Disable the script if player transform is missing
            return;
        }

        if (enemySpawner == null)
        {
            Debug.LogError("Enemy Spawner not assigned in LevelGenerator!");
            enabled = false; // Disable the script if no spawner
            return;
        }

        // Generate initial chunks
        lastChunkEndPosition = transform.position; // Start generation from the generator's position
        for (int i = 0; i < chunksToGenerateAhead; i++)
        {
            GenerateNextChunk();
        }
    }

    void LateUpdate() // Use LateUpdate to ensure player movement is complete
    {
        // Check if the player is close to the end of the currently assumed chunk
        if (playerTransform.position.x > lastChunkEndPosition.x - (chunkLength * chunksToGenerateAhead))
        {
            GenerateNextChunk();
            CheckAndDestroyOldChunks(); // Check and destroy old chunks
        }
    }

    private void GenerateNextChunk()
    {
        if (levelChunkPrefabs == null || levelChunkPrefabs.Length == 0)
        {
            Debug.LogWarning("No level chunk prefabs available to generate!");
            return;
        }

        // Select a random chunk prefab
        int randomIndex = Random.Range(0, levelChunkPrefabs.Length);
        GameObject chunkPrefab = levelChunkPrefabs[randomIndex];

        if (chunkPrefab == null)
        {
            Debug.LogWarning("Selected chunk prefab is null!");
            return;
        }

        // Instantiate the new chunk
        // Assuming chunks are placed side-by-side along the X axis
        GameObject newChunk = Instantiate(chunkPrefab, lastChunkEndPosition, Quaternion.identity);

        // Update the position of the last generated chunk
        lastChunkEndPosition += Vector3.right * chunkLength;

        activeChunks.Add(newChunk); // Add the new chunk to the list of active chunks

        Debug.Log("Generated chunk at " + newChunk.transform.position);

        // Get the ChunkDataHolder from the instantiated chunk and pass the data to the EnemySpawner
        ChunkDataHolder chunkDataHolder = newChunk.GetComponent<ChunkDataHolder>();
        if (chunkDataHolder != null)
        {
            // Pass the generation data to the EnemySpawner
            enemySpawner.StartChunkWaves(chunkDataHolder.chunkData);
        }
        else
        {
            Debug.LogWarning("Chunk prefab " + chunkPrefab.name + " is missing ChunkDataHolder component!");
        }
    }

    private void CheckAndDestroyOldChunks()
    {
        // Iterate through the list of active chunks backwards to safely remove elements
        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = activeChunks[i];
            // Check if the chunk is far behind the player
            if (chunk.transform.position.x < playerTransform.position.x - destroyChunkDistance)
            {
                activeChunks.RemoveAt(i); // Remove from list
                Destroy(chunk); // Destroy the GameObject
                Debug.Log("Destroyed old chunk.");
            }
        }
    }

}