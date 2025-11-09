using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.currentRoom != null)
        {
            Vector3 targetPosition = new Vector3(player.currentRoom.transform.position.x, player.currentRoom.transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        }
    }
}
