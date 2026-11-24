using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }
}