csharp
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class LevelManager : MonoBehaviour // Changed class name to LevelManager (if it was GameManager)
{
    [SerializeField]
    // Removed reference to EnemySpawner as it's not directly used for score

    // Scoring System Variables
    private int score = 0; // Player's current score
    [SerializeField]
    private TextMeshProUGUI scoreText; // Assign a TextMeshProUGUI element in the Inspector for the score display

    void Start()
    {
        // Initialize score and update UI
        UpdateScoreUI();
    }

    // New method to add score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        Debug.Log("Score added: +" + amount + ". Total Score: " + score);
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