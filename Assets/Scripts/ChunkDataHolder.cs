csharp
using UnityEngine;

// Assuming GenerationData.cs defines ChunkGenerationData, WaveConfig, and SpawnPointConfig
// using YourNamespace.GenerationData; // Uncomment and replace with your actual namespace if needed

public class ChunkDataHolder : MonoBehaviour
{
    [SerializeField]
    public ChunkGenerationData generationData; // Configure the waves and spawn points for this chunk in the Inspector
}