using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Transform Player;
    public float speed = 4f;
    int health = 3;
    float healthhold = 0f;
    private SpriteRenderer spriteRenderer;
    public Sprite FullHp;
    public Sprite Halfhp;
    public Rigidbody2D ourbody;
    public float Maxdist = 3;
    bool playernear = false;
    public int randmin;
    public int randmax;
    void Start()
    {
        
        health = Random.Range(randmin, randmax);
        healthhold = health;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = FullHp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playernear == true)
        {
            Vector3 displacement = Player.position - transform.position;
            displacement = displacement.normalized;
            if (Vector2.Distance(transform.position, Player.position) < Maxdist)
            {
                transform.position += (displacement * speed * Time.deltaTime);
            }
            else
            {
                transform.position += (displacement * speed * Time.deltaTime);
            }

            ourbody.velocity = new Vector3(Mathf.Clamp(displacement.x, -2f, 2f), Mathf.Clamp(displacement.y, -4, 4), 0);

        }
        else
        {

        }
        if (health <= healthhold / 2)
        {
            spriteRenderer.sprite = Halfhp;
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }
    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, Player.position) <= 4f)
        {
            playernear = true;
        }
        else
        {
            playernear = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if(collision.collider.tag == "Arrow")
        {
            health = health - 1;
            
        }
    }

}
