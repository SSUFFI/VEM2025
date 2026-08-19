using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Scriptable Object/Item Database",
    fileName = "ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();

    public ItemSO FindById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        return items.Find(
            x => x != null &&
                 x.itemId == itemId);
    }
}