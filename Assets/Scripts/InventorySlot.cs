using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int index;
    public PlayerInventory playerInventory;

    public void OnClick()
    {
        playerInventory.CellClicked(index);
    }

}
