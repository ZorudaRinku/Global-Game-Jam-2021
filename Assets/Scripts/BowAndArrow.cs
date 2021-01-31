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

        if (rightx != 0 || righty != 0)
        {
            animator.SetFloat("Horizontal", rightx);
            animator.SetFloat("Vertical", righty);
            animator.SetFloat("Shoot", 1f);
        }
        else
        {
            animator.SetFloat("Shoot", 0f);
        }
        
        if ((rightx != 0 || righty != 0) && fireFrame)
        {
            fireFrame = false;
            var movement = this.GetComponent<Player>().movement;
            var x = movement.x;
            var y = movement.y;
            speed = Vector2.zero;
            arrow = Instantiate(Arrow);
            var rotation = 0;
            Vector2 offset = new Vector2(0, 0);
            
            if (rightx < 0) //Left
            {
                speed.x = -1.5f;
                speed.y = y / 2;
                rotation = 0;
                offset.x = -this.GetComponent<Renderer>().bounds.size.x/2;
            }
            else if (rightx > 0) //Right
            {
                speed.x = 1.5f;
                speed.y = y / 2;
                rotation = 180;
                offset.x = this.GetComponent<Renderer>().bounds.size.x/2;
            }
            else if (righty < 0) //Down
            {
                speed.y = -1.5f;
                speed.x = x / 2;
                rotation = 90;
                offset.y = -this.GetComponent<Renderer>().bounds.size.y/2;
            }
            else if (righty > 0) //Up
            {
                speed.y = 1.5f;
                speed.x = x / 2;
                rotation = -90;
                offset.y = this.GetComponent<Renderer>().bounds.size.y/2 + 0.2f;
            }

            if (arrow)
            {
                arrow.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, this.transform.position.z);
                arrow.transform.eulerAngles = new Vector3(arrow.transform.eulerAngles.x, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.x + rotation);
                arrow.GetComponent<ArrowPhysics>().speed = speed;
                Destroy(arrow, 5f);
            }
        }
    }

    public void set()
    {
        fireFrame = true;
    }
}
