csharp
using UnityEngine;

// Define a struct to hold configuration for a single spawn point within a chunk/wave
[System.Serializable]
public struct SpawnPointConfig
{
    public Vector3 position; // Position relative to the chunk's origin
}

// Define a struct to hold configuration for a single wave within a chunk
[System.Serializable]
public struct WaveConfig
{
    public GameObject enemyPrefab; // The type of enemy to spawn in this wave
    public int enemyCount; // How many enemies of this type to spawn in this wave
    public float spawnDelay; // Delay between spawning each enemy in this wave
    public SpawnPointConfig[] spawnPointsForWave; // Specific spawn points for this wave within the chunk
}

// Define a struct to hold all generation data for a single level chunk prefab
[System.Serializable]
public struct ChunkGenerationData
{
    public string chunkID; // Optional: A unique identifier for this chunk type
    public WaveConfig[] wavesInChunk; // Array of waves to go through in this chunk
    public float timeBeforeFirstWave; // Delay before the first wave starts after chunk generation
}