using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        if (player.transform.position.y < this.transform.position.y)
        {
            this.transform.position = new Vector3(pos.x, pos.y, -1);
        }
        else
        {
            this.transform.position = new Vector3(pos.x, pos.y, -1.1f);
        }
    }
}
