using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Dictionary<Vector2Int, Vector2Int> doors = new Dictionary<Vector2Int, Vector2Int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Door Generation
        // Loop through all wall tiles and randomly place doors based on doorCount

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player has entered the room.");
        }
    }
}
