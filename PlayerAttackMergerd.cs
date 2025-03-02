using UnityEngine;
using System.Collections;  // Add this line to use IEnumerator and coroutines

public class PlayerAttackMerge : MonoBehaviour
{
    public Animator animator;  // Reference to Animator for animations
    public float moveSpeed = 5f; // Movement speed
    private int swingIndex = 0; // Track the current swing state
    public LayerMask enemyLayers; // Enemy layers to detect

    public KeyCode attackKey = KeyCode.Space; // Key for punch attack (Space key)
    public KeyCode swingKey = KeyCode.B; // Key for sword swing attack (B key)
    public Transform attackPoint; // Attack point to detect enemies
    public float attackRange = 1f; // Range of the attack
    public int attackDamage = 10;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Automatically get Animator if not set
        }
    }

    void Update()
    {
        // Handle punch attack input (Space key)
        if (Input.GetKeyDown(attackKey)) // Check if attack key (Space) is pressed
        {
            PerformPunch();
        }

        // Handle sword swing input (B key)
        if (Input.GetKeyDown(swingKey)) // Check if swing key (B) is pressed
        {
            PerformSwordSwing();
        }
        // Stop animation when the attack key is released
        if (Input.GetKeyUp(attackKey))
        {
            StopAttack();
        }
    }

    // Perform punch animation
    private void PerformPunch()
    {
        // Trigger the punch animation
        animator.SetTrigger("Attack");
        Debug.Log("Punch Triggered");

        // Return to idle state after punch (optional based on timing)
        animator.SetTrigger("idle");
    }

    // Perform sword swing animation and return to idle after it's finished
    private void PerformSwordSwing()
    {
        // Start the swing animation based on swingIndex
        switch (swingIndex)
        {
            case 0:
                animator.SetTrigger("Swing1 0");
                break;
            case 1:
                animator.SetTrigger("Swing2 0");
                break;
            case 2:
                animator.SetTrigger("Swing3 0");
                break;
        }

        // Cycle through the swing states (0, 1, 2)
        swingIndex = (swingIndex + 1) % 3;

        // Start the coroutine to wait for animation completion and then go to idle
        StartCoroutine(SwingAnimationComplete());

        // Detect enemies in attack range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            // Deal damage to enemies (assuming they have a 'TakeDamage' method)
            Debug.Log("Hit " + enemy.name);
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage); // Assuming enemy has a TakeDamage method
        }
    }

    // Coroutine to wait until the animation is finished before going to idle
    private IEnumerator SwingAnimationComplete()
    {
        // Wait for the length of the current swing animation
        float animationLength = 0f;

        // Find out the length of the current animation based on the trigger name
        switch (swingIndex)
        {
            case 0:
                animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                break;
            case 1:
                animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                break;
            case 2:
                animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                break;
        }

        // Wait for the animation to finish before returning to idle
        yield return new WaitForSeconds(animationLength);

        // Return to idle state after the swing
        animator.SetTrigger("idle");
    }

    private void StopAttack()
    {
        // Reset animation triggers or stop all animations
        animator.ResetTrigger("Swing1 0");
        animator.ResetTrigger("Swing2 0");
        animator.ResetTrigger("Swing3 0");

        Debug.Log("Attack Stopped");
    }

    private void OnDrawGizmos()
    {
        // Visualize the attack range in the Scene view (optional)
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
