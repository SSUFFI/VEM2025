using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Shop Item", fileName = "ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public string itemId;
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public int price;
}