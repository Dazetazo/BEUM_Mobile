csharp
using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro

public class EnemySpawner : MonoBehaviour
{

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

    // Removed Start and Update methods (they will be driven by StartChunkWaves call)
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
             Debug.Log("Chunk " + currentChunkData.chunkID + " has no waves defined.");
             // If a chunk has no waves, it's effectively cleared immediately.
             // You might want to trigger the chunk cleared event here.
         }

         StartCoroutine(WaitBeforeFirstWaveInChunk(currentChunkData.timeBeforeFirstWave));
     }

    private IEnumerator WaitBeforeFirstWaveInChunk(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentChunkData.wavesInChunk != null && currentChunkData.wavesInChunk.Length > 0)
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

    {
        enemiesRemainingInChunk--;
        Debug.Log("Enemy defeated in chunk. Enemies remaining in wave: " + enemiesRemainingInChunk);

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

        if (currentWaveIndexInChunk < currentChunkData.wavesInChunk.Length)
            StartCoroutine(SpawnWave(currentChunkData.wavesInChunk[currentWaveIndexInChunk]));
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform spawnPoint in spawnPoints)
            {
                 // Gizmos might be less relevant for a central spawner with chunk-relative points
                 // However, if the spawner itself has a fixed position and defines a local space,
                 // or if spawnPoints array is still used for some reason, keep it.
                 if (spawnPoint != null) // Add null check here
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.5f);
                }
            }
        }
    }
}