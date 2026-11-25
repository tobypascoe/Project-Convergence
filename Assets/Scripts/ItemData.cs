using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int id;
    public GameObject prefab;
    public int maxStack;
    public Sprite image;
    public Vector3 handScale;
}