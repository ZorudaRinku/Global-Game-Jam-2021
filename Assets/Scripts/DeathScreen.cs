using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject deathScreenImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to the player death event
        Player.OnPlayerDeath += HandlePlayerDeath;

        // Make sure the canvas is disabled at start
        if (deathScreenImage != null)
        {
            deathScreenImage.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        // Enable the Canvas child object
        if (deathScreenImage != null)
        {
            deathScreenImage.SetActive(true);
            deathScreenImage.GetComponent<Animator>().SetBool("Dead", true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
