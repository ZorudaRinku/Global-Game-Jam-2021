using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public AudioSource audioSource;
    public float moveSpeed = 5f;
    public float attackSpeed = 5f;
    public float health = 3f;
    public int stars = 0;
    public Rigidbody2D rb;
    public Vector2 movement;
    public Animator animator;
    public float hitTimer = 0f;
    public Image deathScreen;

    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    private Image Star1Image;
    private Image Star2Image;
    private Image Star3Image;
    public Sprite[] Images;

    // Start is called before the first frame update
    void Start()
    {
        Star1Image = Star1.GetComponent<Image>();
        Star2Image = Star2.GetComponent<Image>();
        Star3Image = Star3.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (!animator.GetBool("Death"))
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        Star1Image.sprite = Images[Mathf.Clamp(stars, 0, 5)];
        Star2Image.sprite = Images[Mathf.Clamp(stars + 1, 6, 13)];
        Star3Image.sprite = Images[Mathf.Clamp(stars + 2, 14, 23)];

        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Stump")
        {
            //AudioManager.Instance.PlayOneShot(SoundEffect.Star);
            Destroy(other.transform.GetChild(0).gameObject);
            stars++;
        }

        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            health -= 0.5f;
            Destroy(other.gameObject);
            //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerHurt);
            if (health <= 0)
            {
                animator.SetBool("Death", true);
                //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerDeath);
                //deathScreen.enabled = true;
            }
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
                animator.SetBool("Death", true);
                //AudioManager.Instance.PlayOneShot(SoundEffect.PlayerDeath);
                //deathScreen.enabled = true;
            }

            collision.gameObject.GetComponent<SlimeEnemy>().bounceback((Vector2)(transform.position - collision.transform.position).normalized);
        }
    }
}
