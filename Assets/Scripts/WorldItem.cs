using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public int itemID;
    public bool isConsumable;
    public StatChange effect;

    public StatChange Consume()
    {
        return effect;
    }
}