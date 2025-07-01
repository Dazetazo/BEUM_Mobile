csharp
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class LevelManager : MonoBehaviour // Changed class name to LevelManager (if it was GameManager)
{
    [SerializeField]
    private EnemySpawner enemySpawner; // Assign the EnemySpawner in the Inspector

    // Score Variables
    private int score = 0; // Player's current score
    [SerializeField]
    private TextMeshProUGUI scoreText; // Assign a TextMeshProUGUI element in the Inspector for the score display
    {
        if (enemySpawner == null)
        {
            Debug.LogError("Enemy Spawner not assigned to Level Manager!");
            return;
        }

        // Initialize score and update UI
        UpdateScoreUI();

        // The first area's waves will be triggered by the first AreaTrigger
    }    
    void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks when the Level Manager is destroyed
        // The EnemySpawner no longer has OnAreaCleared event in this endless runner version
        // If a new event like OnChunkWavesCompleted is added and LevelManager subscribes, unsubscribe here.
    }

    // New method to add score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        Debug.Log("Score: " + score);
    }

    // Update the score UI text
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}