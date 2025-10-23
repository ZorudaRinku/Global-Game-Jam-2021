using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : MonoBehaviour
{

    private bool death;
    [SerializeField] private bool deathAnimationFinished;

    [Header("State Machine Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float idleSpeed = 1f;

    [Header("Attack Settings")]
    [SerializeField] private float lungeForce = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private SlimeEnemyState currentState;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator animator;
    private float wanderCooldown;
    [SerializeField] private bool pendingBounceBack;
    private Vector2 bounceBackDirection;
    private float bouncebackAgentRestoreTimer;

    public enum SlimeEnemyState
    {
        Idle,
        ChasePlayer
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player GameObject has the 'Player' tag.");
        }

        currentState = SlimeEnemyState.Idle;
        agent.speed = idleSpeed;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0); // Keep enemy upright

        if (deathAnimationFinished)
        {
            Destroy(this.gameObject);
            return;
        }

        if (death)
        {
            agent.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            rb.simulated = false;
            animator.SetBool("Death", true);
            return;
        }

        if (pendingBounceBack)
        {
            if (agent.enabled)
            {
                agent.enabled = false;
            }
            Debug.Log("Slime Enemy: Performing bounce back!");
            rb.simulated = true;
            rb.AddForce(-bounceBackDirection * lungeForce, ForceMode2D.Impulse);
            bouncebackAgentRestoreTimer = 0.5f;
            pendingBounceBack = false;
            return;
        }

        if (bouncebackAgentRestoreTimer > 0f)
        {
            bouncebackAgentRestoreTimer -= Time.deltaTime;
        }

        if (!agent.enabled && bouncebackAgentRestoreTimer <= 0f)
        {
            pendingBounceBack = false;
            rb.linearDamping = 0.925f;
            ReEnableNavMesh();
            return;
        }

        if (player == null) return;

        // Flip the sprite on Y axis based on direction facing 
        if (rb.linearVelocity.x > 0.1f || agent.velocity.x > 0.1f)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else if (rb.linearVelocity.x < -0.1f || agent.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State machine logic
        switch (currentState)
        {
            case SlimeEnemyState.Idle:
                HandleIdleState(distanceToPlayer);
                break;

            case SlimeEnemyState.ChasePlayer:
                HandleChaseState(distanceToPlayer);
                break;
        }
    }

    private void HandleIdleState(float distanceToPlayer)
    {
        wanderCooldown -= Time.deltaTime;
        if (wanderCooldown <= 0f)
        {
            // Wander around randomly
            agent.ResetPath();
            agent.SetDestination(transform.position + Random.insideUnitSphere * 10f);
            wanderCooldown = Random.Range(6f, 10f);
        }

        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            TransitionToState(SlimeEnemyState.ChasePlayer);
        }
    }

    private void HandleChaseState(float distanceToPlayer)
    {
        // Check if player is too far (lost sight)
        if (distanceToPlayer > detectionRange)
        {
            TransitionToState(SlimeEnemyState.Idle);
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Player is within attack range - stop chasing and attack
            if (agent.enabled)
            {
                agent.ResetPath(); // Stop moving
            }

            // Try to attack if we can attack
            if (CanAttack())
            {
                PerformLungeAttack();
                lastAttackTime = Time.time;
            }
        }

        // Only set destination if agent is enabled and active
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    private void TransitionToState(SlimeEnemyState newState)
    {
        // Exit current state
        switch (currentState)
        {
            case SlimeEnemyState.Idle:
                // No cleanup needed for idle
                break;

            case SlimeEnemyState.ChasePlayer:
                // No cleanup needed for chase
                break;
        }

        // Enter new state
        currentState = newState;

        switch (newState)
        {
            case SlimeEnemyState.Idle:
                agent.speed = idleSpeed;
                Debug.Log("Slime Enemy: Entering Idle state");
                break;

            case SlimeEnemyState.ChasePlayer:
                agent.speed = chaseSpeed;
                Debug.Log("Slime Enemy: Entering Chase state");
                break;
        }
    }

    private bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }

    private void PerformLungeAttack()
    {
        // Lunge towards the player by applying force
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Temporarily disable NavMeshAgent to allow physics-based movement
        agent.enabled = false;

        // Temporarily lower rigidbody drag to allow smoother lunge
        rb.linearDamping = 0f;

        // Apply force towards the player
        rb.AddForce(directionToPlayer * lungeForce, ForceMode2D.Impulse);

        Debug.Log("Slime Enemy is lunging at player!");

        // Re-enable NavMeshAgent after a short delay
        Invoke(nameof(ReEnableNavMesh), 0.5f);
    }

    private void ReEnableNavMesh()
    {
        if (!death && !pendingBounceBack && rb.linearVelocity.magnitude < 1f)
        {
            // Re-enable the agent
            agent.enabled = true;

            // Ensure the agent is properly placed on the NavMesh
            if (!agent.isOnNavMesh)
            {
                // Try to warp to current position to get back on NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
        }
    }

    // Getter for current state (useful for debugging or other scripts)
    public SlimeEnemyState GetCurrentState()
    {
        return currentState;
    }

    // Optional: Draw gizmos to visualize ranges in the Scene view
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Chase range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void bounceback(Vector2 direction)
    {
        bounceBackDirection = direction;
        pendingBounceBack = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            // Add health system if needed
            Destroy(collision.gameObject);
            death = true;
        }
    }
}
