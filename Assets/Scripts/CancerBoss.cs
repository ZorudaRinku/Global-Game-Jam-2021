using UnityEngine;
using UnityEngine.AI;

public class CancerBoss : MonoBehaviour
{
    [SerializeField] private int health = 35;
    private SpriteRenderer spriteRenderer;
    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject starPrefab;

    [Header("Movement Settings")]
    [SerializeField] private float strafeAmount = 3f; // How far to strafe sideways
    [SerializeField] private float strafeChangeInterval = 2f; // How often to change strafe direction
    private float strafeTimer = 0f;
    private int strafeDirection = 1; // 1 for right, -1 for left

    private bool death = false;
    [SerializeField] private bool deathAnimationFinished = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Configure for 2D: disable 3D rotation, we'll handle Z rotation manually
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude >= 0.01f)
        {
            animator.SetBool("Walk", true);
        }

        // Keep enemy on Z=-1 plane
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        // Rotate boss to face movement direction (on Z-axis for 2D)
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            if (spriteRenderer.sprite.name == "crabwalkside_1")
            {
                angle -= 20f;
            }
            else
            {
                angle += 20f;
            }
            // Slerp to smooth rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);
        }

        if (health <= 0 && !death)
        {
            death = true;
            // Play death animation
            animator.SetBool("Death", true);
            Instantiate(starPrefab, transform.position, Quaternion.identity);
            agent.isStopped = true;
            return;
        }

        if (death && deathAnimationFinished)
        {
            Destroy(this.gameObject);
            return;
        }

        // Update strafe timer
        strafeTimer += Time.deltaTime;
        if (strafeTimer >= strafeChangeInterval)
        {
            strafeTimer = 0f;
            strafeDirection *= -1; // Flip strafe direction
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 15f)
        {
            // Calculate direction to player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            // Calculate perpendicular vector for strafing (rotate 90 degrees)
            Vector3 strafeOffset = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0) * strafeAmount * strafeDirection;

            // Set destination to a point that's offset sideways from the player
            Vector3 strafeDestination = player.transform.position + strafeOffset;
            agent.SetDestination(strafeDestination);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Arrow")
        {
            health = health - 1;
            collision.gameObject.GetComponent<ArrowPhysics>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().simulated = false;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

            // Parent first, then set local position
            collision.gameObject.transform.SetParent(this.transform);
            collision.gameObject.transform.position = new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.transform.position.z);

            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4; // Ensure arrow is rendered below
        }
    }
}
