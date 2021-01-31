using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackSpeed = 5f;
    public float health = 3f;
    public int stars = 0;
    public Rigidbody2D rb;
    public Vector2 movement;
    public Animator animator;

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
        
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        Star1Image.sprite = Images[Mathf.Clamp(stars, 0, 5)];
        Star2Image.sprite = Images[Mathf.Clamp(stars + 1, 6, 13)];
        Star3Image.sprite = Images[Mathf.Clamp(stars + 2, 14, 23)];
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Stump")
        {
            Destroy(other.transform.GetChild(0).gameObject);
            stars++;
        }
    }
    
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            health = health - .5f;

        }
    }
}
