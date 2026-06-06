using System.Collections.Generic;
using UnityEngine;

public enum InventoryCategory
{
    All,
    Consumable,
    Material,
    Note
}

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Inst;

    [Header("UI")]
    [SerializeField] Transform content;
    [SerializeField] GameObject itemUIPrefab;

    InventoryCategory currentCategory = InventoryCategory.All;

    void Awake()
    {
        Inst = this;
    }

    void OnEnable()
    {
        Refresh();
    }

    public void ShowAll()
    {
        currentCategory = InventoryCategory.All;
        Refresh();
    }

    public void ShowConsumables()
    {
        currentCategory = InventoryCategory.Consumable;
        Refresh();
    }

    public void ShowMaterials()
    {
        currentCategory = InventoryCategory.Material;
        Refresh();
    }

    public void ShowNotes()
    {
        currentCategory = InventoryCategory.Note;
        Refresh();
    }

    public void Refresh()
    {
        if (content == null || itemUIPrefab == null)
            return;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        if (InventoryManager.Inst == null)
            return;

        List<InventoryItem> items = InventoryManager.Inst.items;

        foreach (InventoryItem invenItem in items)
        {
            if (invenItem == null || invenItem.item == null)
                continue;

            if (!IsMatchCategory(invenItem.item))
                continue;

            GameObject obj = Instantiate(itemUIPrefab, content);
            obj.GetComponent<InventoryItemUI>().Setup(invenItem);
        }
    }

    bool IsMatchCategory(ItemSO item)
    {
        if (currentCategory == InventoryCategory.All)
            return true;

        return item.itemType.ToString() == currentCategory.ToString();
    }
}