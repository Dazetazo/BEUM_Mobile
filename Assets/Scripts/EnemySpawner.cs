csharp
using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro

public class EnemySpawner : MonoBehaviour
{ // Added closing brace here
    // Removed fixed areaWaves array
    [SerializeField]
    private float timeBetweenWaves = 5f;
    [SerializeField]
    private float timeBetweenAreas = 10f; // Time to wait before the player can proceed (conceptually)

    private ChunkGenerationData currentChunkData; // Data for the waves of the current chunk
    private int currentWaveIndexInChunk = 0;
    private int enemiesRemainingToSpawnInWave;
    private int enemiesRemainingInChunk;

    // Event that other scripts can subscribe to when an area is cleared
    // Changed to notify when a chunk's waves are cleared
    public event System.Action<string> OnChunkWavesCompleted;

    // Removed Start and Update methods (they will be driven by StartChunkWaves call), OnDestroy retained
    // Removed PlayerEnteredArea, StartAreaWaves, StartNextWaveInArea, WaitBeforeNextWave, WaitBeforeNextArea (managed by LevelManager/Generator)
    // Retained OnDestroy

     void OnDestroy()
    {
         // Note: Event handler OnChunkWavesCompleted needs to be managed by the subscriber (LevelManager)
         // This spawner doesn't subscribe to events, it raises them.
    }

    // New method called by LevelGenerator (or Chunk script) to start waves for a generated chunk
    public void StartChunkWaves(ChunkGenerationData data)
     {
         if (currentChunkData.wavesInChunk != null && currentChunkData.wavesInChunk.Length > 0) // Check if spawner is already busy
         {
             // This check is a bit simplistic. Maybe check currentWaveIndexInChunk or enemiesRemainingInChunk.
             // For now, let's allow overwriting if a new chunk's data is explicitly passed.
         }
         currentChunkData = data;
         
         currentWaveIndexInChunk = 0; // Start from the first wave in the new chunk

         // Initialize enemy counts for the first wave
         if (currentChunkData.wavesInChunk != null && currentChunkData.wavesInChunk.Length > 0)
         {
             WaveConfig firstWave = currentChunkData.wavesInChunk[currentWaveIndexInChunk];
             // Calculate total enemies in the chunk to track remaining enemies
             enemiesRemainingInChunk = 0; // Reset for the new chunk
             foreach(WaveConfig wave in currentChunkData.wavesInChunk)
             {
                 enemiesRemainingInChunk += wave.enemyCount;
             }
             // We don't need enemiesRemainingToSpawnInWave tracked here anymore, only total for the chunk
         }
         else
         {
             Debug.Log("Chunk " + currentChunkData.chunkID + " has no waves defined.");
             // If a chunk has no waves, it's effectively cleared immediately.
             // You might want to trigger the chunk cleared event here.
         }

         StartCoroutine(WaitBeforeFirstWaveInChunk(currentChunkData.timeBeforeFirstWave));
     }

     private IEnumerator WaitBeforeFirstWaveInChunk(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentChunkData.wavesInChunk != null && currentChunkData.wavesInChunk.Length > 0 && currentWaveIndexInChunk < currentChunkData.wavesInChunk.Length)
           StartCoroutine(SpawnWave(currentChunkData.wavesInChunk[currentWaveIndexInChunk]));
    }

     private IEnumerator SpawnWave(WaveConfig wave)
     {
         for (int i = 0; i < wave.enemyCount; i++)
         {
             if (wave.spawnPointsForWave != null && wave.spawnPointsForWave.Length > 0)
             {
                 // Select a random spawn point for this wave
                 SpawnPointConfig randomSpawnPoint = wave.spawnPointsForWave[Random.Range(0, wave.spawnPointsForWave.Length)];

                 // Instantiate the enemy at the spawn point's position relative to the spawner's position
                 // Use the ObjectPooler to get an enemy instance instead of Instantiate
                 // Assuming ObjectPooler is properly set up and has enemies
                 if (ObjectPooler.Instance != null && ObjectPooler.Instance.prefabToPool != null) // Basic check
                 {
                     GameObject spawnedEnemy = ObjectPooler.Instance.SpawnFromPool(transform.position + randomSpawnPoint.position, Quaternion.identity);

                     // Enemy.cs will call OnEnemyDefeatedInChunk when destroyed/returned to pool
                 }
             }
         }

         // Spawning for this wave is complete.
         // The logic to move to the next wave is in OnEnemyDefeatedInChunk
     }
     // Added the OnEnemyDefeatedInChunk method definition and fixed structure
     public void OnEnemyDefeatedInChunk()
     {
        enemiesRemainingInChunk--;

        if (enemiesRemainingInChunk <= 0)
        {
            Debug.Log("All enemies in chunk defeated. Chunk waves completed: " + currentChunkData.chunkID);
            // Notify listeners that the chunk's waves are completed
            if (OnChunkWavesCompleted != null)
            {
                OnChunkWavesCompleted(currentChunkData.chunkID);
                // Reset chunk data after completion
                currentChunkData = new ChunkGenerationData();
            }
            // You might want to also trigger the start of the next chunk's waves here
            // Or the LevelGenerator will handle calling StartChunkWaves for the next one
        } else {
             // If not all enemies are defeated, check if the *current* wave is done spawning
             // This logic seems to be missing. We need to track enemies spawned per wave vs killed per wave.
        }
    }

    private void ChunkWavesCompleted()
    {
        Debug.Log("All enemies in chunk defeated. Chunk waves completed: " + currentChunkData.chunkID);
        // Notify listeners that the chunk's waves are completed
        if (OnChunkWavesCompleted != null)
        {
            OnChunkWavesCompleted(currentChunkData.chunkID);
            // Reset chunk data after completion so StartChunkWaves can be called again
            currentChunkData = new ChunkGenerationData();
            currentWaveIndexInChunk = 0; // Reset wave index
        }
    }

    private IEnumerator WaitBeforeNextWaveInChunk()
    {
        yield return new WaitForSeconds(timeBetweenWaves); // Use timeBetweenWaves between waves
        currentWaveIndexInChunk++; // Move to the next wave in the current chunk

        if (currentChunkData.wavesInChunk != null && currentWaveIndexInChunk < currentChunkData.wavesInChunk.Length)
        {
            StartCoroutine(SpawnWave(currentChunkData.wavesInChunk[currentWaveIndexInChunk]));
        }
        else
        {
            // No more waves in this chunk. All enemies in chunk are considered spawned now.
            // Wait for remaining enemies to be defeated (handled by OnEnemyDefeatedInChunk)
            Debug.Log("No more waves to spawn in chunk " + currentChunkData.chunkID + ". Waiting for remaining enemies.");
        }
    }

    void OnDrawGizmosSelected()
    {
        // The original spawnPoints array variable was removed.
        // This Gizmo code needs to be adapted or removed if no longer relevant.
        // If you want to visualize the spawn points of the *current* chunk's active wave:
        if (currentChunkData.wavesInChunk != null && currentWaveIndexInChunk < currentChunkData.wavesInChunk.Length)
        {
            Gizmos.color = Color.blue;
            foreach (SpawnPointConfig spawnPoint in currentChunkData.wavesInChunk[currentWaveIndexInChunk].spawnPointsForWave)
            {
                 if (spawnPoint != null) // Add null check here
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.5f);
                }
            }
        }
    }
}