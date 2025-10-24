using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.z = transform.position.z; // Maintain the camera's z position
            transform.position = newPosition;
        }
    }
}
