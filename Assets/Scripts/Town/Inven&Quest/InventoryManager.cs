using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemSO item;
    public int count;

    public InventoryItem(ItemSO item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Inst;

    public List<InventoryItem> items = new List<InventoryItem>();

    [Header("Test")]
    [SerializeField] ItemSO testItem;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Add Test Item")]
    public void AddTestItem()
    {
        AddItem(testItem, 1);
    }

    public void AddItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return;

        InventoryItem exist = items.Find(x => x.item == item);

        if (exist != null)
        {
            exist.count += amount;
        }
        else
        {
            items.Add(new InventoryItem(item, amount));
        }

        RefreshInventoryUI();
    }

    public void RemoveItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return;

        InventoryItem exist = items.Find(x => x.item == item);

        if (exist == null)
            return;

        exist.count -= amount;

        if (exist.count <= 0)
            items.Remove(exist);

        RefreshInventoryUI();
    }

    public int GetItemCount(ItemSO item)
    {
        if (item == null)
            return 0;

        InventoryItem exist = items.Find(x => x.item == item);

        return exist != null ? exist.count : 0;
    }

    public bool TryRemoveItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        InventoryItem exist = items.Find(x => x.item == item);

        if (exist == null || exist.count < amount)
            return false;

        exist.count -= amount;

        if (exist.count <= 0)
            items.Remove(exist);

        RefreshInventoryUI();

        return true;
    }

    void RefreshInventoryUI()
    {
        if (InventoryUI.Inst != null)
            InventoryUI.Inst.Refresh();
    }
}