using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class itemID
{
    public string itemName;
    public int idNum;
    public GameObject item;
}


public class PlayerInventory : MonoBehaviour
{
    private List<int> hotbar = new List<int>(new int[10]);
    private List<int> inventory = new List<int>(new int[20]);
    
    public List<itemID> itemList;
    public Image[] hotbarImages;

    private int activeHotbarSlot = 0;

    void addItem(List<int> activeList, int index, int itemID)
    {
        activeList[index] = itemID;
    }
}