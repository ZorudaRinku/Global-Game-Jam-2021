using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy2 : MonoBehaviour
{

    public Transform Player;
    public float speed = 0.25f;
    public int health = 2;
    private SpriteRenderer spriteRenderer;
    public Sprite FullHp;
    public Sprite Halfhp;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = FullHp;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 displacement = Player.position - transform.position;
        displacement = displacement.normalized;
        if(Vector2.Distance(Player.position, transform.position) > 0)
        {
            transform.position += (displacement * speed * Time.deltaTime);
        }
        else
        {
            transform.position += (displacement * speed * Time.deltaTime);
        }
        if (health == 0)
        {
            Destroy(gameObject);
        }
        else if (health == 1)
        {
            spriteRenderer.sprite = Halfhp;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(collision.collider.tag == "Arrow")
        {
            health--;
        }
    }

}
