using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BowAndArrow : MonoBehaviour
{
    public GameObject Arrow;
    public Rigidbody2D rb;
    Vector2 speed;
    public float cooldown = 1f;
    private float timeStamp;

    // Start is called before the first frame update
    void Start()
    {
        timeStamp = Time.time + cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject arrow = null;
        var rightx = Input.GetAxisRaw("Right X");
        var righty = Input.GetAxisRaw("Right Y");
        if ((rightx != 0 || righty != 0) && (timeStamp <= Time.time))
        {
            var movement = this.GetComponent<PlayerMovement>().movement;
            var x = movement.x;
            var y = movement.y;
            speed = Vector2.zero;
            arrow = Instantiate(Arrow);
            var rotation = 0;
            Vector2 offset = new Vector2(0, 0);
            
            if (rightx < 0) //Left
            {
                speed.x = -1f + x/2;
                speed.y = y / 2;
                rotation = 0;
                offset.x = -this.GetComponent<Renderer>().bounds.size.x/2;
            }
            else if (rightx > 0) //Right
            {
                speed.x = 1f + x/2;
                speed.y = y / 2;
                rotation = 180;
                offset.x = this.GetComponent<Renderer>().bounds.size.x/2;
            }
            else if (righty < 0) //Down
            {
                speed.y = -1f + y/2;
                speed.x = x / 2;
                rotation = 90;
                offset.y = -this.GetComponent<Renderer>().bounds.size.y/2;
            }
            else if (righty > 0) //Up
            {
                speed.y = 1f + y/2;
                speed.x = x / 2;
                rotation = -90;
                offset.y = this.GetComponent<Renderer>().bounds.size.y/2;
            }

            if (arrow)
            {
                timeStamp = Time.time + cooldown;
                arrow.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, this.transform.position.z);
                arrow.transform.eulerAngles = new Vector3(arrow.transform.eulerAngles.x, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.x + rotation);
                arrow.GetComponent<ArrowPhysics>().speed = speed;
                Destroy(arrow, 5f);
            }
        }
    }
}
