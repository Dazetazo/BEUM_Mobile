csharp
using UnityEngine;

// Assuming GenerationData.cs defines ChunkGenerationData, WaveConfig, and SpawnPointConfig
// using YourNamespace.GenerationData; // Uncomment and replace with your actual namespace if needed if structs are in a namespace



public class ChunkDataHolder : MonoBehaviour
{
    [SerializeField]
    public ChunkGenerationData chunkData; // Configure the waves and spawn points for this chunk in the Inspector
}