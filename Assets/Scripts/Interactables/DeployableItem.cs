using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployableItem : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory playerInventory)
    {
        playerInventory.Deploy();
    }
}
