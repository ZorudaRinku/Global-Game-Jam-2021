using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAndArrow : MonoBehaviour
{
    public GameObject Arrow;
    public Rigidbody2D rb;
    Vector2 speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GameObject arrow;
            arrow = Instantiate(Arrow);

            var movement = this.GetComponent<PlayerMovement>().movement;
            var x = movement.x;
            var y = movement.y;
            speed = Vector2.zero;
            if (x < 0)
            {
                speed.x = -1f + x;
            }
            
            if (x > 0)
            {
                speed.x = 1f + x;
            }
            
            if (y < 0)
            {
                speed.y = -1f + y;
            }
            
            if (y > 0)
            {
                speed.y = 1f + y;
            }
            
            if (x == 0 && y == 0)
            {
                speed.y = -1f;
                speed.x = 0f;
            }
            Debug.Log(speed);
            arrow.transform.position = this.transform.position;
            arrow.GetComponent<ArrowPhysics>().speed = speed;
            Destroy(arrow, 5f);
        }    
    }
}
