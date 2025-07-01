csharp
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField]
    private int maxHealth = 5; // Maximum health of the enemy 
    private int currentHealth; // Current health of the enemy

    // Cache the spawner and level manager to avoid repeated FindObjectOfType calls
    private EnemySpawner enemySpawner;
    private LevelManager levelManager; 

    void Awake()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        enemySpawner = FindObjectOfType<EnemySpawner>(); // Get reference in Awake
        levelManager = FindObjectOfType<LevelManager>(); // Get reference in Awake
    }

    // Method to call when the enemy receives damage
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Reduce health by damage amount
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // Call method when health drops to 0 or below
        }
    }

    // Method called when the enemy is defeated
    private void Die()
    {
        Debug.Log(gameObject.name + " was defeated. Returning to pool.");

        // Notify the EnemySpawner that this enemy has been defeated using the cached reference
        if (enemySpawner != null)
        {
            enemySpawner.OnEnemyDefeatedInChunk();
        } 

        // Add score via the LevelManager using the cached reference
        if (levelManager != null)
        {
            levelManager.AddScore(50); // Example: add 50 points (adjust as needed)
        }

        // Implement enemy defeat logic here
        // Disable renderer and colliders temporarily if needed for death animation 

        ResetState(); // Reset state before returning to the pool

        ObjectPooler.Instance.ReturnToPool(gameObject); // Return the enemy to the pool
    }
    
    // Method to reset the enemy's state for reuse
    public void ResetState() 
    {
        currentHealth = maxHealth; // Reset health
        // Reset other relevant states here, e.g.:
        // Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // if (rb != null) rb.velocity = Vector2.zero; // Stop movement
        // Animator anim = GetComponent<Animator>();
        // if (anim != null) anim.Play("Idle"); // Reset animation state
        // Enable renderer and colliders if they were disabled for death animation
        // GetComponent<Renderer>().enabled = true;
        // GetComponent<Collider2D>().enabled = true;
    }
}