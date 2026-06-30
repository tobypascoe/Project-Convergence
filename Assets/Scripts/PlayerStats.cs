using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private float health = 100f;

    private float hunger = 100f;
    private float hungerLossRate = 0.1f;

    private float thirst = 100f;
    private float thirstLossRate = 0.1f;

    void Update()
    {
        ChangeHunger(-hungerLossRate * Time.deltaTime);
        ChangeThirst(-thirstLossRate * Time.deltaTime);

        Debug.Log("Hunger = " + hunger + ", thirst = " + thirst);
    }

    public void ChangeHealth(float change)
    {
        if (health <= 0)
        {
            // implement death
            return; 
        }
        health += change;
    }

    public void ChangeHunger(float change)
    {
        if (hunger <= 0)
        {
            // implement starving
            return; 
        }
        hunger += change;
    }

    public void ChangeThirst(float change)
    {
        if (thirst <= 0)
        {
            // implement dying from thirst
            return; 
        }
        thirst += change;
    }
}