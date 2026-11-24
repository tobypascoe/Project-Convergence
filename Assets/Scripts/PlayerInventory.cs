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
    public ItemData data;
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
    public int currHotbar => activeHotbarSlot;

    public Transform hand;

    public GameObject currentHeldItem;

    public PlayerStats playerStats;
    public PlayerMovement playerMovement;
    public ItemDatabase itemDB;

    public Sprite emptyHotbar;
    public GameObject openedInventory;
    private bool isInventoryOpen = false;
    private bool isMovingItem = false;
    private int movingItemID;
    public Image movingImage;
    public RectTransform movingIcon;
    private int movingStack = 0;
    public TextMeshProUGUI movingCount;
    public Deploying deploying;

    void Awake()
    {
        for (int i = 0; i < 40; i++)
        {
            inventory[i] = new InventoryArray();
            inventory[i].data = null;
            inventory[i].stack = 0;
        }
        if (itemDB.GetItem(2) == null)
        {
            Debug.Log("youf SADOF");
        }
        AddItem(itemDB.GetItem(2));
        AddItem(itemDB.GetItem(2));
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

    public void AddItem(ItemData data, int amount=1)
    {
        int nextEmpty = -1;
        for (int i = 0; i < 40; i++)
        {
            if (inventory[i].data == null) // find the lowest value empty stack
            {
                if (nextEmpty == -1)
                    nextEmpty = i;
                continue;
            }
            else if (inventory[i].data.id == data.id && inventory[i].stack < data.maxStack) // check to find existing stack of same type
            {
                inventory[i].stack += amount;
                UpdateCountText(i);
                return;
            }
        }

        if (nextEmpty > -1)
        {
            inventory[nextEmpty].data = data;
            inventory[nextEmpty].stack++;
            if (nextEmpty < 10)
            {
                if (data == null)
                {
                    Debug.Log("data null");
                }
                else if (data.image == null)
                {
                    Debug.Log("data image null");
                }
                hotbarComponents[nextEmpty].hotbarImage.sprite = data.image;
                UpdateCountText(nextEmpty);
            }
            else
            {
                inventoryComponents[nextEmpty - 10].inventoryImage.sprite = data.image;
            }
        }
        else
            Debug.Log("Inventory full");
    }

    public void RemoveItem(int index, int amount=1)
    {
        inventory[index].stack -= amount;
        UpdateCountText(activeHotbarSlot);
        if (inventory[index].stack == 0)
        {
            inventory[index].data = null;
            if (index < 10)
            {
                hotbarComponents[index].hotbarImage.sprite = emptyHotbar;
            }
        }
    }

    public void ChangeActiveHotbar()
    {
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
        }

        int itemID = inventory[activeHotbarSlot].data.id;

        if (itemID == 0)
        {
            return;
        }
        currentHeldItem = Instantiate(itemDB.GetItem(itemID).prefab, hand);
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
        else
        {
            if (inventory[index].stack == 0)
            {
                inventoryComponents[index - 10].count.text = "";
            }
            else
            {
                inventoryComponents[index - 10].count.text = inventory[index].stack.ToString();
            }
        }
    }

    public void Interaction()
    {
        IInteractable interactable = currentHeldItem.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact(this);
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

        ItemData movingItem = itemDB.GetItem(movingItemID);

        if (!isMovingItem)
        {
            if (inventory[index].stack > 0) // picking up item
            {
                isMovingItem = true;
                movingItemID = inventory[index].data.id;
                movingStack = inventory[index].stack;
                movingCount.text = movingStack.ToString();
                inventory[index].data.id = 0;
                inventory[index].stack = 0;

                if (index >= 10)
                {
                    inventoryComponents[index - 10].inventoryImage.sprite = emptyHotbar;
                    UpdateCountText(index);
                }
                else
                {
                    hotbarComponents[index].hotbarImage.sprite = emptyHotbar;
                    UpdateCountText(index);
                }
                    
                movingImage.sprite = movingItem.image;
                movingIcon.gameObject.SetActive(true);
            }
        }
        else
        {
            if (inventory[index].data.id == 0) // moving to empty slot
            {
                inventory[index].data.id = movingItemID;
                inventory[index].stack = movingStack;

                if (index >= 10)
                {
                    inventoryComponents[index - 10].inventoryImage.sprite = movingItem.image;
                    UpdateCountText(index);
                }
                else
                {
                    hotbarComponents[index].hotbarImage.sprite = movingItem.image;
                    UpdateCountText(index);
                }
                resetMove();
            }
            else if (inventory[index].data.id == movingItemID) // moving to slot with same item
            {
                if (inventory[index].stack < movingItem.maxStack) 
                {
                    if (inventory[index].stack + movingStack < movingItem.maxStack) // add all of moving item to slot
                    {
                        inventory[index].stack += movingStack;
                        movingCount.text = movingStack.ToString();
                        UpdateCountText(index);
                        resetMove();
                    }
                    else // add as much of moving item to slot as possible
                    { 
                        int diff = itemDB.GetItem(movingItemID).maxStack - inventory[index].stack;
                        inventory[index].stack += diff;
                        UpdateCountText(index);
                        movingStack -= diff;
                        movingCount.text = movingStack.ToString();
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

    public GameObject GetHeldItem()
    {
        return currentHeldItem;
    }

    public void Deploy()
    {
        deploying.StartDeploy();
    }
}