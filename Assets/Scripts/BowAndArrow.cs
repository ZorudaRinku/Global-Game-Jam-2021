using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowAndArrow : MonoBehaviour
{
    public GameObject Arrow;
    public Rigidbody2D rb;
    Vector2 speed;
    private float timeStamp;
    private GameObject Player;
    private Player PlayerScript;
    public Animator animator;
    private bool fireFrame = false;

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        GameObject arrow;
        var rightx = Input.GetAxisRaw("Right X");
        var righty = Input.GetAxisRaw("Right Y");

        bool isShooting = (rightx != 0 || righty != 0);

        // Lock shooting to 4 cardinal directions
        float shootX = 0f;
        float shootY = 0f;

        if (isShooting)
        {
            // Prioritize the axis with larger input
            if (Mathf.Abs(rightx) > Mathf.Abs(righty))
            {
                // Horizontal dominant
                shootX = rightx > 0 ? 1f : -1f;
                shootY = 0f;
            }
            else
            {
                // Vertical dominant
                shootX = 0f;
                shootY = righty > 0 ? 1f : -1f;
            }
        }

        // Only control shooting layer parameters
        animator.SetBool("IsShooting", isShooting);

        if (isShooting)
        {
            animator.SetFloat("ShootHorizontal", shootX);
            animator.SetFloat("ShootVertical", shootY);
        }

        if (isShooting && fireFrame)
        {
            fireFrame = false;
            var movement = this.GetComponent<Player>().movement;
            var x = movement.x;
            var y = movement.y;
            arrow = Instantiate(Arrow);
            var rotation = 0;
            Vector2 offset = new Vector2(0, 0);

            if (shootX < 0) //Left
            {
                speed.x = -1.5f;
                speed.y = y / 2;
                rotation = 0;
                offset.x = -this.GetComponent<Renderer>().bounds.size.x / 2;
            }
            else if (shootX > 0) //Right
            {
                speed.x = 1.5f;
                speed.y = y / 2;
                rotation = 180;
                offset.x = this.GetComponent<Renderer>().bounds.size.x / 2;
            }
            else if (shootY < 0) //Down
            {
                speed.y = -1.5f;
                speed.x = x / 2;
                rotation = 90;
                offset.y = -this.GetComponent<Renderer>().bounds.size.y / 2;
            }
            else if (shootY > 0) //Up
            {
                speed.y = 1.5f;
                speed.x = x / 2;
                rotation = -90;
                offset.y = this.GetComponent<Renderer>().bounds.size.y / 2;
            }

            if (arrow)
            {
                arrow.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, this.transform.position.z);
                arrow.transform.eulerAngles = new Vector3(arrow.transform.eulerAngles.x, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.x + rotation);
                arrow.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shootX, shootY) * 10f;
                //Destroy(arrow, 5f);
            }
        }

    }

    public void set()
    {
        fireFrame = true;
    }
}
