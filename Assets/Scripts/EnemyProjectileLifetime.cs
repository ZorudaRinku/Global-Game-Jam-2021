using UnityEngine;

public class EnemyProjectileLifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f;
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            // Lerp scale down to zero over 0.2 seconds before destroying
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.zero, Time.deltaTime * 5f);
            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
