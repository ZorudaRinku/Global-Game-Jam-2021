using UnityEngine;
using UnityEngine.AI;

public class EyeEnemy : MonoBehaviour
{
    [Header("State Machine Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float attackCooldown = 2f;

    private bool death;
    [SerializeField] private bool deathAnimationFinished;
    
    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float idleSpeed = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectile2Prefab;
    
    private NavMeshAgent agent;
    private Transform player;
    private EyeEnemyState currentState;
    private float lastAttackTime;

    [SerializeField] private float boltSpeedMultiplier = 1f;

    private float health = 1f;
    
    public enum EyeEnemyState
    {
        Idle,
        ChasePlayer
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player")?.transform;
        
        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player GameObject has the 'Player' tag.");
        }
        
        currentState = EyeEnemyState.Idle;
        agent.speed = idleSpeed;
    }
    
    void Awake()
    {
    }

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
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Animator>().SetBool("Death", true);
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State machine logic
        switch (currentState)
        {
            case EyeEnemyState.Idle:
                HandleIdleState(distanceToPlayer);
                break;

            case EyeEnemyState.ChasePlayer:
                HandleChaseState(distanceToPlayer);
                break;
        }
    }
    
    private void HandleIdleState(float distanceToPlayer)
    {
        // Stop moving
        agent.ResetPath();
        
        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            TransitionToState(EyeEnemyState.ChasePlayer);
        }
    }
    
    private void HandleChaseState(float distanceToPlayer)
    {
        // Check if player is too far (lost sight)
        if (distanceToPlayer > detectionRange)
        {
            TransitionToState(EyeEnemyState.Idle);
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Player is within attack range - stop chasing and attack
            agent.ResetPath(); // Stop moving

            // Try to attack if we can attack
            if (CanAttack())
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = player.position - directionToPlayer * chaseRange;
        agent.SetDestination(targetPosition);
    }
    
    private void TransitionToState(EyeEnemyState newState)
    {
        // Exit current state
        switch (currentState)
        {
            case EyeEnemyState.Idle:
                // No cleanup needed for idle
                break;
                
            case EyeEnemyState.ChasePlayer:
                // No cleanup needed for chase
                break;
        }
        
        // Enter new state
        currentState = newState;
        
        switch (newState)
        {
            case EyeEnemyState.Idle:
                agent.speed = idleSpeed;
                Debug.Log("Eye Enemy: Entering Idle state");
                break;
                
            case EyeEnemyState.ChasePlayer:
                agent.speed = chaseSpeed;
                Debug.Log("Eye Enemy: Entering Chase state");
                break;
        }
    }
    
    private bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
    
    private void PerformAttack()
    {
        // TODO: Implement actual attack logic here
        // Instantiate a projectile towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -directionToPlayer);
        GameObject bolt = Instantiate(projectilePrefab, transform.position, rotation) as GameObject;
        bolt.GetComponent<Rigidbody2D>().linearVelocity = (directionToPlayer * 5f) * boltSpeedMultiplier; // Set projectile speed and add our velocity
        Debug.Log("Eye Enemy is attacking!");
    }
    
    // Getter for current state (useful for debugging or other scripts)
    public EyeEnemyState GetCurrentState()
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            health -= 0.5f;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                death = true;
            }
        }
    }
}
