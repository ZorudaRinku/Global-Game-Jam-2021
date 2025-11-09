using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Delegate and event for player death
    public delegate void PlayerDeathHandler();
    public static event PlayerDeathHandler OnPlayerDeath;

    // Delegate and event for star collection
    public delegate void StarCollectedHandler(int totalStars);
    public static event StarCollectedHandler OnStarCollected;

    public AudioSource audioSource;
    public float moveSpeed = 5f;
    public float attackSpeed = 5f;
    public float health = 3f;
    public int stars = 0;
    public Rigidbody2D rb;
    public Vector2 movement;
    public Animator animator;
    public float hitTimer = 0f;
    private Image Star1Image;
    private Image Star2Image;
    private Image Star3Image;
    public Sprite[] Images;

    private Vector2 lastDirection = Vector2.down; // Default facing direction
    public GameObject currentRoom;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetBool("Death"))
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Store last direction when moving
            if (movement.sqrMagnitude > 0.01f)
            {
                lastDirection = movement.normalized;
            }

            // Use last direction when idle, current movement when moving
            Vector2 animDirection = movement.sqrMagnitude > 0.01f ? movement : lastDirection;

            animator.SetFloat("Horizontal", animDirection.x);
            animator.SetFloat("Vertical", animDirection.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!animator.GetBool("Death"))
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Die()
    {
        animator.SetBool("Death", true);
        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;
        OnPlayerDeath?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star") && other.transform.childCount > 0)
        {
            //AudioManager.Instance.PlayOneShot(SoundEffect.Star);
            Destroy(other.transform.GetChild(0).gameObject);
            stars++;
            OnStarCollected?.Invoke(stars);
        }

        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            health -= 0.5f;
            Destroy(other.gameObject);
            //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerHurt);
            if (health <= 0)
            {
                Die();
            }
        }

        if (other.gameObject.CompareTag("Room"))
        {
            currentRoom = other.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy" && hitTimer <= 0)
        {
            hitTimer = 1f; // 1 second invulnerability
            health -= 1f;
            //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerHurt);
            if (health <= 0)
            {
                Die();
            }

            collision.gameObject.GetComponent<SlimeEnemy>().bounceback((Vector2)(transform.position - collision.transform.position).normalized);
        }

        if (collision.collider.CompareTag("Boss") && hitTimer <= 0)
        {
            hitTimer = 1f; // 1 second invulnerability
            health -= 1f;
            //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerHurt);
            if (health <= 0)
            {
                Die();
            }
        }
    }
}
