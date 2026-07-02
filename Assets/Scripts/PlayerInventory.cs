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
    public GameObject openedInventory;
    private bool isInventoryOpen = false;
    private bool isMovingItem = false;
    public Image movingImage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenInventory();
        }

        for (int i = 0; i < hotbarComponents.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                activeHotbarSlot = i;
                ChangeActiveHotbar();
            }
        }
        if (isMovingItem)
        {
            movingImage.rectTransform.position = Input.mousePosition + new Vector3(20, -20, 0);
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
                UpdateCountText(i);
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
                UpdateCountText(nextEmpty);
            }
            Destroy(itemObject);
        }
    }

    private void RemoveItem(int index, int amount=1)
    {
        inventoryStacks[index] -= amount;
        UpdateCountText(activeHotbarSlot);
        if (inventoryStacks[index] == 0)
        {
            inventoryID[index] = 0;
            if (index < 10)
            {
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

    private void UpdateCountText(int index)
    {
        if (index < 10)
        {
            if (inventoryStacks[index] == 0)
            {
                hotbarComponents[index].count.text = "";
            }
            else
            {
                hotbarComponents[index].count.text = inventoryStacks[index].ToString();
            }
        }
    }

    public void Interact()
    {
        WorldItem worldItem = currentHeldItem.GetComponent<WorldItem>();

        if (worldItem.isConsumable)
        {
            playerStats.ConsumedItem(worldItem.Consume());
            RemoveItem(activeHotbarSlot);
            if (inventoryStacks[activeHotbarSlot] == 0)
            {
                Destroy(currentHeldItem);
            }
        }
    }

    private void OpenInventory()
    {
        if (!isInventoryOpen)
        {
            openedInventory.SetActive(true);
            isInventoryOpen = true;
        }
        else
        {
            openedInventory.SetActive(false);
            isInventoryOpen = false;
        }
        
    }

    public void CellClicked(int index)
    {
        if (!isMovingItem)
        {
            
        }
        else
        {

        }
        
    }
}