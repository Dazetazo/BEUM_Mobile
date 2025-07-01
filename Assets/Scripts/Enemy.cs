csharp
using UnityEngine;

public class Enemy : MonoBehaviour {

    // --- Health ---
    [SerializeField]
    private int maxHealth = 5; // Maximum health of the enemy 
    private int currentHealth; // Current health of the enemy

    // Cache the spawner and level manager to avoid repeated FindObjectOfType calls
    private EnemySpawner enemySpawner;
    private LevelManager levelManager; 

    // --- Movement ---
    [SerializeField] private float moveSpeed = 2f;
    private Transform playerTransform; // Cache the player's transform
    private Rigidbody2D rb;

    // --- Attack ---
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    // --- Animation ---
    private Animator animator;


    void Awake()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        // Get references in Awake
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

    void Start()
    {
        // Get references in Start to avoid issues with script execution order if needed
        enemySpawner = FindObjectOfType<EnemySpawner>();
        levelManager = FindObjectOfType<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Find the player - should be tagged "Player"
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (playerGameObject != null)
        {
            playerTransform = playerGameObject.transform;
        }
        else
        {
             Debug.LogWarning("Player GameObject with tag 'Player' not found in the scene.");
        }

         lastAttackTime = -attackCooldown; // Allow immediate attack at start
    }

    void FixedUpdate()
    {
        // Physics-based movement and attack checks
        if (playerTransform == null) return; // Don't do anything if player isn't found

        MoveTowardsPlayer();
        CheckForAttack();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void CheckForAttack()
    {
        // Check if player is within attack range and cooldown is ready
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log(gameObject.name + " attacks Player!");
        // Placeholder for attack logic
        lastAttackTime = Time.time; // Reset cooldown timer
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
        if (rb != null) rb.velocity = Vector2.zero; // Stop movement
        if (animator != null) animator.Play("Idle"); // Reset animation state (assuming "Idle" exists)
        lastAttackTime = -attackCooldown; // Reset attack cooldown

        // Enable renderer and colliders if they were disabled for death animation
        // GetComponent<Renderer>().enabled = true;
        // GetComponent<Collider2D>().enabled = true;

         // Reactivate GameObject if it was deactivated for pooling (ObjectPooler handles this)
    }

    // Opcional: dibujar el rango de detección y ataque en el editor para depuración
    void OnDrawGizmosSelected()
    {
        // Draw Detection Range (optional, if you add it later)
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}