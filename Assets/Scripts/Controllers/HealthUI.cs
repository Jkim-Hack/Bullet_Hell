using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    private int maxHeartAmount = 10;
    public int startHealth = 10;
    public int currHealth;
    private int maxHealth;
    private int healthPerBar = 1;

    public Image[] healthImages;
    public Sprite[] healthSprites;

	// Use this for initialization
	void Start () {

        currHealth = startHealth * healthPerBar;
        maxHealth = maxHeartAmount * healthPerBar;
        checkHealthAmount();
	}
	
	
	void checkHealthAmount()
    {
        for(int i = 0; i < maxHeartAmount; i++)
        {
            if(startHealth <= i)
            {
                healthImages[i].enabled = false;
            } else
            {
                healthImages[i].enabled = true;
            }
        }
        UpdateHearts();
    }
    void UpdateHearts()
    {
        bool empty = false;
        int i = 0;

        foreach(Image image in healthImages)
        {
            if (empty)
            {
                image.sprite = healthSprites[0];
            } else
            {
                i++;
                if(currHealth >= i * healthPerBar)
                {
                    image.sprite = healthSprites[1];
                } else
                {
                    int currentHealthBar = (int)(healthPerBar - (healthPerBar * i - currHealth));
                    int healthPerImage = healthPerBar / (healthSprites.Length - 1);
                    int imageIndex = currentHealthBar / healthPerImage;
                    image.sprite = healthSprites[imageIndex];
                    empty = true;

                }
            }
        }


    }

    public void TakeDamage(int amount)
    {
        currHealth += amount;
        currHealth = Mathf.Clamp(currHealth, 0, startHealth * healthPerBar);
        UpdateHearts();
    }

    public void AddHeartContainer()
    {
        startHealth++;
        startHealth = Mathf.Clamp(startHealth, 0, maxHeartAmount);
        checkHealthAmount();
    }
}
