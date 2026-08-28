using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : MonoBehaviour, IInteractable
{
    public StatChange effect;

    public void Interact(PlayerInventory playerInventory)
    {
        playerInventory.playerStats.ConsumedItem(effect);
        playerInventory.RemoveItem(playerInventory.currHotbar);
    }
}
