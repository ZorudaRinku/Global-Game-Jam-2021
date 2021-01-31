using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomControl : MonoBehaviour
{
    private BoxCollider2D bc;
    // Start is called before the first frame update
    void Start()
    {
        bc = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Tilemap child = this.transform.GetChild(i).GetComponent<Tilemap>();
            //Tilemap.SetColor(, new Color(26, 26, 26, 255));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        throw new NotImplementedException();
    }
}
