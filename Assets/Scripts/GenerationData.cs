csharp
using UnityEngine;

[System.Serializable]
public struct SpawnPointConfig
{
    public Vector3 position; // Position relative to the chunk's origin
}

[System.Serializable]
public struct WaveConfig
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnDelay;
    public SpawnPointConfig[] spawnPointsForWave;
}

[System.Serializable]
public struct ChunkGenerationData
{
    public string chunkID;
    public WaveConfig[] wavesInChunk;
    public float timeBeforeFirstWave;
}


// Note: Struct definitions should not be inside a class or namespace
// unless you specifically want them to be nested.
// They are correctly defined at the top level here.