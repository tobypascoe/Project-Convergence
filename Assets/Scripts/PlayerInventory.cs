using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ItemID
{
    public string itemName;
    public int idNum;
    public GameObject item;
    public int maxStack;
    public Sprite itemImage;
}

[System.Serializable]
public class HotbarComponent
{
    public Image hotbarImage;
    public TextMeshProUGUI count;
}

[System.Serializable]
public class InventoryComponent
{
    public Image inventoryImage;
    public TextMeshProUGUI count;
}

public class InventoryArray
{
    public int ID;
    public int stack;
}

public class PlayerInventory : MonoBehaviour
{
    private List<int> inventoryStacks = new List<int>(new int[40]);

    private InventoryArray[] inventory = new InventoryArray[40];

    public List<ItemID> itemList;
    public List<HotbarComponent> hotbarComponents;
    public List<InventoryComponent> inventoryComponents;

    private int activeHotbarSlot = 0;
    public Transform hand;

    public GameObject currentHeldItem;

    public PlayerStats playerStats;
    public PlayerMovement playerMovement;

    public Sprite emptyHotbar;
    public GameObject openedInventory;
    private bool isInventoryOpen = false;
    private bool isMovingItem = false;
    private int movingItemID;
    public Image movingImage;
    public RectTransform movingIcon;
    private int movingStack = 0;
    public TextMeshProUGUI movingCount;

    void Start()
    {
        for (int i = 0; i < 40; i++)
        {
            inventory[i] = new InventoryArray();
            inventory[i].ID = 0;
            inventory[i].stack = 0;
        }
    }

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
            movingIcon.position = Input.mousePosition + new Vector3(20, -20, 0);
        }
    }

    public void AddItem(int itemID, GameObject itemObject)
    {
        int nextEmpty = -1;
        for (int i = 0; i < 40; i++)
        {
            if (inventory[i].ID == itemID && inventory[i].stack < itemList[itemID-1].maxStack)
            {
                inventory[i].stack++;
                Destroy(itemObject);
                UpdateCountText(i);
                return;
            }

            if (inventory[i].ID == 0 && nextEmpty == -1) nextEmpty = i;
        }
        
        if (nextEmpty != -1 && inventory[nextEmpty].stack < itemList[itemID-1].maxStack)
        {
            inventory[nextEmpty].ID = itemID;
            inventory[nextEmpty].stack++;
            if (nextEmpty < 10)
            {
                hotbarComponents[nextEmpty].hotbarImage.sprite = itemList[itemID-1].itemImage;
                UpdateCountText(nextEmpty);
            } 
            else
            {
                inventoryComponents[nextEmpty-10].inventoryImage.sprite = itemList[itemID - 1].itemImage;
            }
            Destroy(itemObject);
        }
    }

    private void RemoveItem(int index, int amount=1)
    {
        inventory[index].stack -= amount;
        UpdateCountText(activeHotbarSlot);
        if (inventory[index].stack == 0)
        {
            inventory[index].ID = 0;
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

        int itemID = inventory[activeHotbarSlot].ID;

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
            if (inventory[index].stack == 0)
            {
                hotbarComponents[index].count.text = "";
            }
            else
            {
                hotbarComponents[index].count.text = inventory[index].stack.ToString();
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
            if (inventory[activeHotbarSlot].stack == 0)
            {
                Destroy(currentHeldItem);
            }
        }
    }

    private void OpenInventory()
    {
        if (!isInventoryOpen)
        {
            playerMovement.InInventory(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            openedInventory.SetActive(true);
            isInventoryOpen = true;
        }
        else
        {
            playerMovement.InInventory(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            openedInventory.SetActive(false);
            isInventoryOpen = false;
        }
        
    }

    public void CellClicked(int index)
    {
        if (!isMovingItem)
        {
            if (inventory[index].stack > 0)
            {
                isMovingItem = true;
                movingItemID = inventory[index].ID;
                movingStack = inventory[index].stack;
                movingCount.text = movingStack.ToString();
                inventory[index].ID = 0;
                inventory[index].stack = 0;

                if (index > 10) inventoryComponents[index - 10].inventoryImage.sprite = emptyHotbar;
                else
                {
                    hotbarComponents[index].hotbarImage.sprite = emptyHotbar;
                    UpdateCountText(index);
                }
                    
                movingImage.sprite = itemList[movingItemID-1].itemImage;
                movingIcon.gameObject.SetActive(true);
            }
        }
        else
        {
            if (inventory[index].ID == 0)
            {
                inventory[index].ID = movingItemID;
                inventory[index].stack = movingStack;

                if (index > 10) inventoryComponents[index-10].inventoryImage.sprite = itemList[movingItemID-1].itemImage;
                else
                {
                    hotbarComponents[index].hotbarImage.sprite = itemList[movingItemID - 1].itemImage;
                    UpdateCountText(index);
                }
                resetMove();
            }
            else if (inventory[index].ID == movingItemID)
            {
                if (inventory[index].stack < itemList[movingItemID - 1].maxStack)
                {
                    if (inventory[index].stack + movingStack < itemList[movingItemID].maxStack)
                    {
                        inventory[index].stack += movingStack;
                        UpdateCountText(index);
                        resetMove();
                    }
                    else
                    {
                        int diff = itemList[movingItemID].maxStack - inventory[index].stack;
                        inventory[index].stack += diff;
                        UpdateCountText(index);
                        movingStack -= diff;
                    }
                }
            }
        }
    }

    void resetMove()
    {
        movingItemID = 0;
        movingStack = 0;
        isMovingItem = false;
        movingIcon.gameObject.SetActive(false);
    }
}