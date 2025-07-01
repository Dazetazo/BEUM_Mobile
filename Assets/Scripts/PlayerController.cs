csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
 {
 [Header("Movement")]
 [SerializeField] private float moveSpeed = 5f;
 private Vector2 moveInput;
 private Rigidbody2D rb; // Using Rigidbody2D for 2D movement
 private Animator animator;
 private bool isDefeated = false; // Added for defeat state

 [Header("Health")]
 [SerializeField] private int maxHealth = 100; // Maximum health of the player
 private int currentHealth;

 [Header("Attack")]
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb; // Using Rigidbody2D for 2D movement
 [SerializeField] private float attackRange = 1f; // Distance of attack
 [SerializeField] private int attackDamage = 20; // Damage dealt by the player
 [SerializeField] private LayerMask enemyLayer; // Layer to detect enemies
 private bool isAttacking = false; // Added for attack state

 [Header("Combo System")]
 [SerializeField] private float comboWindow = 0.5f; // Time within which the next attack can continue the combo
 [SerializeField] private int maxCombo = 3; // Maximum number of attacks in a combo sequence (e.g., 3-hit combo)
    [Header("Input")]
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
 animator = GetComponent<Animator>();
 currentHealth = maxHealth; // Initialize current health

 // Combo System Initialization
 comboCount = 0;
 lastAttackTime = -comboWindow; // Initialize to allow combo on first attack


        // Assuming you are using the new Input System
        // You would need to set up your Input Actions asset
        // and assign the appropriate action to moveAction
        // For simplicity, let's assume you have an action named "Move"
        // You would typically link this in the Unity Editor or via code
        // if using a dynamically created Input Action asset.
        // For this basic setup, we'll rely on the Input System handling.

        // Example if using an Input Actions asset:
        // playerInput = GetComponent<PlayerInput>(); // Add a PlayerInput component
        // moveAction = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        // If using Input Action asset:
        // moveAction.Enable();
    }

    private void OnDisable()
    {
        // If using Input Action asset:
        // moveAction.Disable();
    }

    private void Update()
    {
 if (isDefeated) return; // Don't process input or updates if defeated

        // Check if the combo window has expired
        if (Time.time >= lastAttackTime + comboWindow && comboCount > 0)
        {
        // Basic touch input simulation for movement (for testing without a dedicated action)
        // In a real mobile game, you'd likely use a virtual joystick or specific touch gestures
        // This is a placeholder to show how input *could* be handled.
        moveInput = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            // Simple example: move towards the touch position (not ideal for beat 'em up,
            // but demonstrates touch input detection)
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 direction = (touchWorldPos - transform.position).normalized;

            // This is a very basic interpretation; a proper mobile beat 'em up
            // would likely use a virtual joystick UI element to control movement direction.
            // For now, let's simulate directional input with arrow keys/WASD if not on mobile
            // or if using a different input scheme.

            // Let's stick to a more traditional directional input for now,
            // assuming it will be mapped to touch input via an Input Actions asset later.
            // You would read the mapped touch input from your Input Action asset.

            // Placeholder for reading input from an Action (assuming "Move" action provides a Vector2)
            // if (moveAction != null)
            // {
            //     moveInput = moveAction.ReadValue<Vector2>();
            // }
        }
        else
        {
             // Fallback for keyboard/gamepad input during testing
             moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
 }
            ResetCombo(); // Reset combo if time window passed

        // Animation logic (placeholders)
        UpdateAnimations();
    }

 private void UpdateAnimations()
 {
        if (animator == null) return;

        // Simple logic: play walk animation if moving, idle otherwise
        bool isMoving = moveInput.magnitude > 0.1f; // Use a small threshold for floating point inaccuracies
 animator.SetBool("IsWalking", isMoving); // You'll need an "IsWalking" boolean parameter in your Animator Controller
    }

    private void FixedUpdate()
    {
 if (isDefeated)
        {
 rb.velocity = Vector2.zero; // Stop movement if defeated
 return;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        if (rb != null)
        {
            rb.velocity = moveInput.normalized * moveSpeed;
        }
    }

    // Input Action Callbacks (Connect these in the Unity Input System)
    public void OnMove(InputAction.CallbackContext context)
    {
 if (isDefeated) return;
 moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
 if (isDefeated) return;

        // Only allow attack if not already attacking and within combo window OR if combo is reset
        if (context.started && !isAttacking && (Time.time < lastAttackTime + comboWindow || comboCount == 0))
        {
 // Increment combo count, capping at maxCombo
            comboCount = (comboCount % maxCombo) + 1;
 Attack(); // Call the attack method
        }
 // Added logic for attacking outside the combo window
 else if (context.started && !isAttacking && comboCount > 0 && Time.time >= lastAttackTime + comboWindow)
        {
 ResetCombo(); // Reset combo first
            comboCount = 1; // Start a new combo
 Attack(); // Call the attack method
        }
    }

    // Attack Logic
    public void Attack()
    {
 if (isAttacking || isDefeated) return; // Prevent attacking if already attacking or defeated

        isAttacking = true;
 lastAttackTime = Time.time; // Record the time this attack started

        // Trigger the appropriate attack animation based on comboCount
 PlayAttackAnimation(comboCount); // This needs to be implemented based on your animations

        // Trigger the attack animation. You'll need a trigger or bool parameter in your Animator Controller.
        // For example, if you have a "Attack" trigger:
        // animator.SetTrigger("Attack");

        // Delay the enemy detection to match the animation's hit frame(s)
        // In a real game, you'd use animation events for this for precision.
        // The delay might need to be different for different combo hits.
        float attackDetectionDelay = 0.2f; // Default delay - Adjust based on your animation

        Invoke("PerformAttackDetection", attackDetectionDelay);

        // Reset attacking state after a delay that matches the animation length
        // This delay might also need to be different for different combo hits.
        float resetAttackDelay = 0.5f; // Default delay - Adjust based on your animation

        Invoke("ResetAttackState", resetAttackDelay);
    }

    private void PerformAttackDetection()
    {
        if (!isAttacking || isDefeated) return; // Only detect hits if currently attacking

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name + " with combo hit " + comboCount);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage); // Apply damage to the enemy
            }
        }
    }

    private void ResetAttackState()
    {
        isAttacking = false;
    }

    // Combo System Logic
    private int comboCount = 0; // Current number of hits in the combo
    private float lastAttackTime; // Time when the last attack animation started

    private void ResetCombo()
    {
        if (comboCount > 0) // Only log/reset if there was an active combo
        {
 Debug.Log("Combo reset. Combo count was: " + comboCount);
 // You might want to trigger a combo break animation or sound here
        }
        comboCount = 0;
        // You might want to hide the combo UI here if it's displayed
    }

    // Animation Logic (Placeholders)
    private void UpdateAnimations()
    {
        if (animator == null || isAttacking || isDefeated) return; // Don't change walk/idle animation if attacking or defeated

        // Simple logic: play walk animation if moving, idle otherwise
        bool isMoving = moveInput.magnitude > 0.1f; // Use a small threshold for floating point inaccuracies
 // Assuming you have "IsWalking" boolean and "AttackX" trigger/states in your Animator Controller
        // animator.SetBool("IsWalking", isMoving);
    }

    // Placeholder methods for playing specific animations
    private void PlayAttackAnimation(int comboHitNumber)
    {
        if (animator != null)
        {
 // Example: Trigger an animation parameter based on comboHitNumber
 // animator.SetTrigger("Attack" + comboHitNumber); // Assuming you have triggers named "Attack1", "Attack2", etc.
        }
    }

    // Health System Methods
    public void TakeDamage(int damage)
    {
        if (isDefeated) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Defeat();
        }
    }

    public void Heal(int amount)
    {
        if (isDefeated) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent healing above max health
        Debug.Log(gameObject.name + " healed for " + amount + ". Current health: " + currentHealth);
        // You might want to update a health bar UI here
    }

    private void Defeat()
    {
        isDefeated = true;
        Debug.Log(gameObject.name + " was defeated.");
        // Handle defeat state (e.g., play death animation, disable controls, game over)
        // For now, let's simply deactivate the GameObject
        gameObject.SetActive(false);
    }

    // Placeholder for weapon switching logic
    public void ChangeWeapon(GameObject newWeaponPrefab)
    {
 Debug.Log("Weapon changed (placeholder). New weapon prefab: " + (newWeaponPrefab != null ? newWeaponPrefab.name : "None"));
 // Implement logic here to swap weapon visuals, adjust attack damage/range, etc.
    }

    // Placeholder for score accumulation
    public void AddScore(int amount)
    {
 Debug.Log("Score added (placeholder): +" + amount);
 // You would add logic here to update a score variable and potentially a score UI
    }

    // Optional: Draw attack range in the editor for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Ensure the GameObject is selected in the hierarchy to see the gizmo
        // Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}