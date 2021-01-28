using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPhysics : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;

    public Vector2 speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.x <= 0 && rb.velocity.y <= 0)
        {
            //Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + speed * moveSpeed * Time.fixedDeltaTime);
    }
}
