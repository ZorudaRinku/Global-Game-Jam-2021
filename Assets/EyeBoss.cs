using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBoss : MonoBehaviour
{
    int health = 25;

    private bool awake = false;
    private Animator animator;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            animator.SetBool("Death", true);
            Destroy(this.gameObject, 1.5f);
        }
        
        Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if (Vector2.Distance(this.transform.position, player.transform.position) <= 15)
        {
            awake = true;
            animator.SetBool("Walk", true);
        }
        else
        {
            awake = false;
            animator.SetBool("Walk", false);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if(collision.collider.tag == "Arrow")
        {
            health = health - 1;
        }
    }

    public void walk()
    {
        if (awake)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 120 * Time.deltaTime);
            Vector3 vectorToTarget = player.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 5.0f);
        }
    }
}
