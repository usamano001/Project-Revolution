using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform Player; // Reference to the player's transform
    public Animator animator; // Animator to handle animations
    public float attackRange = 2f; // Distance within which the enemy prepares to attack
    public float attackCooldown = 1.5f; // Time between each attack
    public int attackDamage = 10; // Damage dealt to the player
    public Collider leftHandCollider; // Left hand collider
    public Collider rightHandCollider; // Right hand collider
    public bool canMove = false; // Controls whether the enemy can move or attack

    private float lastAttackTime = 0f; // Last time the enemy attacked
    private bool isAttacking = false; // Flag to track if attack is in progress

    void Start()
    {
        // Ensure the colliders are disabled initially
        if (leftHandCollider != null)
            leftHandCollider.enabled = false;

        if (rightHandCollider != null)
            rightHandCollider.enabled = false;

        if (Player == null)
            Debug.LogWarning("Player Transform is not assigned!");
    }

    void Update()
    {
        if (!canMove) // Enemy remains stationary if movement is disabled
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isRetreating", false);
            return;
        }

        if (Player == null) return; // Ensure Player reference is assigned

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        // If within attack range, stop walking/running and attack
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isWalking", false); // Stop walking
            animator.SetBool("isRunning", false); // Stop running
            animator.SetBool("isRetreating", false); // Stop retreating
            animator.SetTrigger("Punching"); // Start the punching animation

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Resume walking or running when out of range
            animator.SetBool("isWalking", true); // Walking animation enabled
            animator.ResetTrigger("Punching"); // Ensure punching animation stops

            // Example logic for running if player is farther away (adjust distance as needed)
            if (distanceToPlayer > attackRange * 1.5f)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true); // Switch to running animation
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", true); // Switch back to walking animation
            }
        }
    }

    void Attack()
    {
        // Play the attack animation
        animator.SetTrigger("Attack");

        // Enable hand colliders at the appropriate time (sync with animation using Animation Events or delay)
        isAttacking = true;
        EnableHandColliders();

        // Disable the colliders after a short delay (sync with animation timing)
        Invoke(nameof(DisableHandColliders), 0.5f); // Adjust the delay to match the attack animation
    }

    void EnableHandColliders()
    {
        if (leftHandCollider != null)
            leftHandCollider.enabled = true;

        if (rightHandCollider != null)
            rightHandCollider.enabled = true;

        Debug.Log("Hand colliders enabled.");
    }

    void DisableHandColliders()
    {
        if (leftHandCollider != null)
            leftHandCollider.enabled = false;

        if (rightHandCollider != null)
            rightHandCollider.enabled = false;

        isAttacking = false;
        Debug.Log("Hand colliders disabled.");
    }

    // Handle collision events for the hand colliders
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage); // Apply damage
                Debug.Log("Player took " + attackDamage + " damage from hand collision!");
            }
        }
    }

    // Method to allow the enemy to move (e.g., when the player exits the canvas)
    public void EnableMovement()
    {
        canMove = true;
        Debug.Log("Enemy movement enabled.");
    }
}
