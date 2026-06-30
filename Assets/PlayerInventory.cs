using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class itemID
{
    public string itemName;
    public int idNum;
    public GameObject item;
    public int maxStack;
    public Sprite itemImage;
}

[System.Serializable]
public class hotbarComponent
{
    public Image hotbarImage;
    public TextMeshProUGUI count;
}

public class PlayerInventory : MonoBehaviour
{
    private List<int> inventory = new List<int>(new int[30]);
    private List<int> inventoryStacks = new List<int>(new int[30]);

    public List<itemID> itemList;
    public List<hotbarComponent> hotbarComponents;

    private int activeHotbarSlot = 0;

    public void addItem(int itemID, GameObject itemObject)
    {
        int nextEmpty = -1;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == itemID && inventoryStacks[i] < itemList[itemID-1].maxStack)
            {
                inventoryStacks[i]++;
                Destroy(itemObject);
                updateCountText(inventoryStacks[i], i);
                return;
            }

            if (inventory[i] == 0 && nextEmpty == -1) nextEmpty = i;
        }
        
        if (nextEmpty != -1 && inventoryStacks[nextEmpty] < itemList[itemID-1].maxStack)
        {
            inventory[nextEmpty] = itemID;
            inventoryStacks[nextEmpty]++;
            if (nextEmpty < 10)
            {
                hotbarComponents[nextEmpty].hotbarImage.sprite = itemList[itemID-1].itemImage;
                updateCountText(inventoryStacks[nextEmpty], nextEmpty);
            }
            Destroy(itemObject);
        }
    }

    private void updateCountText(int stackNum, int hotbarNum)
    {
        if (hotbarNum < 10)
        {
            hotbarComponents[hotbarNum].count.text = stackNum.ToString();
        }
    }
}