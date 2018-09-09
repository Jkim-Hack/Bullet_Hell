using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;

    private float health;
    private float healthPercentage;

    private void Awake()
    {
        health = maxHealth;
        healthPercentage = 100;
    }

    public void takeDamage(int amount)
    {
        health = health - amount;
        calculateHealthPercentage();
        print("Health class health: " + health);
        GUIDelegate.updateHealthUI(this);
    }

    private void calculateHealthPercentage()
    {
        this.healthPercentage = (health / maxHealth) * 100;
    }

    public void setHealth(float health)
    {
        this.health = health;
        calculateHealthPercentage();
    }

    public float getHealth()
    {
        return health;
    }

    public float getHealthPercentage()
    {
        return healthPercentage;
    }



}
