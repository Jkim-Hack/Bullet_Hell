using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthGUIController : MonoBehaviour
{
    public UIUnitFrame_Bar healthBar;
    //public Text healthPercentageText;

    void Awake()
    {
        GUIDelegate.OnHit += damageUpdateOnHealthBar;
        
    }

    /*
    private void Start()
    {
        healthBar = GetComponent<Image>();
    }
    */

    // Update is called once per frame
    private void damageUpdateOnHealthBar(Health healthProperties)
    {
        float health = healthProperties.getHealth();
        print("Health: " + health);
        //float maxHealth = healthProperties.maxHealth;

       // float ratio = health / maxHealth;

        setFillAmount((int)health);
    }
    

    private void setFillAmount(int health)
    {
        this.healthBar.value = (int)health;
    }

    

  
    /*
    private void updateHealthPercentage(Health healthProperties)
    {
        this.healthPercentageText.text = healthProperties.getHealthPercentage() + "%";
    }
    */

}
