using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/ItemSO", fileName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemId;
    public string itemName;
    public Sprite icon;

    [TextArea]
    public string description;

    public int price;

    public ItemType itemType;
}

public enum ItemType
{
    Consumable,
    Material,
    Note
}