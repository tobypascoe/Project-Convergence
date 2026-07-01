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
    private List<int> inventoryID = new List<int>(new int[30]);
    private List<int> inventoryStacks = new List<int>(new int[30]);

    public List<itemID> itemList;
    public List<hotbarComponent> hotbarComponents;

    private int activeHotbarSlot = 0;
    public Transform hand;

    public GameObject currentHeldItem;

    public PlayerStats playerStats;

    public Sprite emptyHotbar;

    void Update()
    {
        for (int i = 0; i < hotbarComponents.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                activeHotbarSlot = i;
                ChangeActiveHotbar();
            }
        }
    }

    public void AddItem(int itemID, GameObject itemObject)
    {
        int nextEmpty = -1;
        for (int i = 0; i < inventoryID.Count; i++)
        {
            if (inventoryID[i] == itemID && inventoryStacks[i] < itemList[itemID-1].maxStack)
            {
                inventoryStacks[i]++;
                Destroy(itemObject);
                UpdateCountText(inventoryStacks[i], i);
                return;
            }

            if (inventoryID[i] == 0 && nextEmpty == -1) nextEmpty = i;
        }
        
        if (nextEmpty != -1 && inventoryStacks[nextEmpty] < itemList[itemID-1].maxStack)
        {
            inventoryID[nextEmpty] = itemID;
            inventoryStacks[nextEmpty]++;
            if (nextEmpty < 10)
            {
                hotbarComponents[nextEmpty].hotbarImage.sprite = itemList[itemID-1].itemImage;
                UpdateCountText(inventoryStacks[nextEmpty], nextEmpty);
            }
            Destroy(itemObject);
        }
    }

    private void RemoveItem(int index, int amount=1)
    {
        inventoryStacks[index] -= amount;
        if (inventoryStacks[index] == 0)
        {
            inventoryID[index] = 0;
            if (index < 10)
            {
                Debug.Log("changing sprite to empty");
                hotbarComponents[index].hotbarImage.sprite = emptyHotbar;
            }
        }
    }

    private void ChangeActiveHotbar()
    {
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
        }

        int itemID = inventoryID[activeHotbarSlot];
        Debug.Log(itemID-1);

        if (itemID == 0)
        {
            return;
        }
        currentHeldItem = Instantiate(itemList[itemID-1].item, hand);
    }

    private void UpdateCountText(int stackNum, int hotbarNum)
    {
        if (hotbarNum < 10)
        {
            hotbarComponents[hotbarNum].count.text = stackNum.ToString();
        }
    }

    public void Interact()
    {
        WorldItem worldItem = currentHeldItem.GetComponent<WorldItem>();

        if (worldItem.isConsumable)
        {
            playerStats.ConsumedItem(worldItem.Consume());
        }
    }
}