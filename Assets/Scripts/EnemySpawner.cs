csharp
using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro

// Use the structs defined in GenerationData.cs
// Removed internal struct definitions: SpawnPointConfig, WaveConfig, ChunkGenerationData


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
         if (currentChunkData.wavesInChunk != null && currentWaveIndexInChunk < currentChunkData.wavesInChunk.Length)
         {
             Debug.LogWarning("EnemySpawner is already handling waves for a chunk. Ignoring new chunk data.");
             return; // Don't start new waves if already busy
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

             enemiesRemainingToSpawnInWave = firstWave.enemyCount;
         }
         else
         {
             Debug.Log("Chunk " + currentChunkData.chunkID + " has no waves defined.");
             // If a chunk has no waves, it's effectively cleared immediately.
             // You might want to trigger the chunk cleared event here.
         }

         StartCoroutine(WaitBeforeFirstWaveInChunk(currentChunkData.timeBeforeFirstWave));
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
                 GameObject spawnedEnemy = ObjectPooler.Instance.SpawnFromPool(transform.position + randomSpawnPoint.position, Quaternion.identity);

                 // Subscribe to enemy death event to track remaining enemies
                 Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
                 if (enemyScript != null)
                 {
                     // Need to pass a reference to this spawner or use a global event system
                     // For now, let's stick to the simplified FindObjectOfType approach but call the new method
                     // enemyScript.OnDeath += OnEnemyDefeatedInChunk; // If using events
                     // FindObjectOfType<EnemySpawner>().OnEnemyDefeatedInChunk(); // If using FindObjectOfType (needs modification in Enemy.cs)
                 }

                 enemiesRemainingToSpawnInWave--;

                 yield return new WaitForSeconds(wave.spawnDelay);
             }
             else
             {
                 Debug.LogWarning("Wave has no spawn points defined!");
                 enemiesRemainingToSpawnInWave = 0; // Ensure spawning loop ends
                 break;
             }
         }

         // Spawning for this wave is complete.
         // The logic to move to the next wave is in OnEnemyDefeatedInChunk
     }

    // This method is called by enemies from the current chunk's waves when they are defeated
    public void OnEnemyDefeatedInChunk()
    {
        enemiesRemainingInChunk--;
        Debug.Log("Enemy defeated in chunk. Enemies remaining in chunk: " + enemiesRemainingInChunk);

        if (enemiesRemainingInChunk <= 0)
        {
            ChunkWavesCompleted();
        }
    }
    public void OnEnemyDefeatedInChunk()
    {
        enemiesRemainingInChunk--;
        Debug.Log("Enemy defeated in chunk. Enemies remaining in wave: " + enemiesRemainingInChunk);

        if (enemiesRemainingInChunk <= 0)
        {
            Debug.Log("All enemies in chunk defeated. Chunk waves completed: " + currentChunkData.chunkID);
            // Notify listeners that the chunk's waves are completed
            if (OnChunkWavesCleared != null)
            {
                OnChunkWavesCleared(currentChunkData.chunkID);
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

    // Event that other scripts can subscribe to when an area is cleared
    // Changed to notify when a chunk's waves are cleared
    public event System.Action<string> OnChunkWavesCleared;

    [SerializeField]
    private Transform[] spawnPoints; // Array of potential spawn points
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