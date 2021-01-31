using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health = 3;
    public int numOfHearts = 3;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                if (i + 1 != math.ceil(health))
                {
                    hearts[i].sprite = fullHeart;
                } else
                {
                    if (health % 1 == 0)
                    {
                        hearts[i].sprite = fullHeart;
                    }
                    else
                    {
                        hearts[i].sprite = halfHeart;
                    }
                }
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            } else {
                hearts[i].enabled = false;
            }
        }
    }
}
