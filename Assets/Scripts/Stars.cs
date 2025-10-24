using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    [SerializeField] private Image Star1Image;
    [SerializeField] private Image Star2Image;
    [SerializeField] private Image Star3Image;
    [SerializeField] private Sprite[] Star1Images;
    [SerializeField] private Sprite[] Star2Images;
    [SerializeField] private Sprite[] Star3Images;

    private int stars = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to the star collection event
        Player.OnStarCollected += UpdateStarDisplay;

        // Initialize the display
        UpdateStarDisplay(0);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        Player.OnStarCollected -= UpdateStarDisplay;
    }

    private void UpdateStarDisplay(int totalStars)
    {
        stars = totalStars;

        Star1Image.sprite = Star1Images[Mathf.Clamp(stars, 0, Star1Images.Length - 1)];
        Star2Image.sprite = Star2Images[Mathf.Clamp(stars - 5, 0, Star2Images.Length - 1)];
        Star3Image.sprite = Star3Images[Mathf.Clamp(stars - 12, 0, Star3Images.Length - 1)];
    }
}
