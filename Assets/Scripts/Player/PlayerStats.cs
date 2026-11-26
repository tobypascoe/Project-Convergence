using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private float health = 100f;
    public RectTransform healthBar;
    private float healthBarWidth = 140f;

    private float hunger = 100f;
    private float hungerLossRate = 5f;
    public RectTransform hungerBar;
    private float hungerBarWidth = 140f;

    private float thirst = 100f;
    private float thirstLossRate = 5f;
    public RectTransform thirstBar;
    private float thirstBarWidth = 140f;

    void Update()
    {
        ChangeHunger(-hungerLossRate * Time.deltaTime);
        ChangeThirst(-thirstLossRate * Time.deltaTime);
    }

    public void ChangeHealth(float change)
    {
        if (health <= 0)
        {
            healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            return; 
        }
        health = Mathf.Clamp(health + change, 0f, 100f);
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidth * health / 100);
    }

    public void ChangeHunger(float change)
    {
        if (hunger <= 0)
        {
            ChangeHealth(-0.2f * Time.deltaTime);
            hungerBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            return; 
        }
        hunger += change;
        hunger = Mathf.Clamp(hunger + change, 0f, 100f);
        hungerBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hungerBarWidth * hunger / 100);
    }

    public void ChangeThirst(float change)
    {
        if (thirst <= 0)
        {
            ChangeHealth(-0.2f * Time.deltaTime);
            thirstBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            return; 
        }
        thirst += change;
        thirst = Mathf.Clamp(thirst + change, 0f, 100f);
        thirstBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thirstBarWidth * thirst / 100);
    }

    public void ConsumedItem(StatChange statChange)
    {
        ChangeHealth(statChange.health);
        ChangeHunger(statChange.hunger);
        ChangeThirst(statChange.thirst);
    }
}