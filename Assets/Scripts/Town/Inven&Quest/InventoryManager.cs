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

[System.Serializable]
public class InventorySaveItem
{
    public string itemId;
    public int count;

    public InventorySaveItem(string itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventorySaveItem> items =
        new List<InventorySaveItem>();
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Inst;

    [Header("Inventory")]
    public List<InventoryItem> items =
        new List<InventoryItem>();

    [Header("Item Database")]
    [SerializeField] ItemDatabaseSO itemDatabase;

    [Header("Test")]
    [SerializeField] ItemSO testItem;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);

            LoadInventory();
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

        InventoryItem exist =
            items.Find(x => x.item == item);

        if (exist != null)
        {
            exist.count += amount;
        }
        else
        {
            items.Add(
                new InventoryItem(
                    item,
                    amount));
        }

        SaveInventory();
        RefreshInventoryUI();
    }

    public void RemoveItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return;

        InventoryItem exist =
            items.Find(x => x.item == item);

        if (exist == null)
            return;

        exist.count -= amount;

        if (exist.count <= 0)
            items.Remove(exist);

        SaveInventory();
        RefreshInventoryUI();
    }

    public int GetItemCount(ItemSO item)
    {
        if (item == null)
            return 0;

        InventoryItem exist =
            items.Find(x => x.item == item);

        return exist != null
            ? exist.count
            : 0;
    }

    public bool TryRemoveItem(
        ItemSO item,
        int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        InventoryItem exist =
            items.Find(x => x.item == item);

        if (exist == null ||
            exist.count < amount)
            return false;

        exist.count -= amount;

        if (exist.count <= 0)
            items.Remove(exist);

        SaveInventory();
        RefreshInventoryUI();

        return true;
    }


    public void SaveInventory()
    {
        string key =
            AccountManager.GetAccountKey(
                "Inventory");

        InventorySaveData saveData =
            new InventorySaveData();

        foreach (InventoryItem inventoryItem in items)
        {
            if (inventoryItem.item == null)
                continue;

            if (string.IsNullOrEmpty(
                    inventoryItem.item.itemId))
            {
                Debug.LogWarning(
                    $"{inventoryItem.item.name}의 itemId가 없습니다.");

                continue;
            }

            saveData.items.Add(
                new InventorySaveItem(
                    inventoryItem.item.itemId,
                    inventoryItem.count));
        }

        string json =
            JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(
            key,
            json);

        PlayerPrefs.Save();
    }


    public void LoadInventory()
    {
        items.Clear();

        if (itemDatabase == null)
        {
            Debug.LogWarning(
                "InventoryManager에 ItemDatabaseSO가 연결되지 않았습니다.");

            RefreshInventoryUI();
            return;
        }

        string key =
            AccountManager.GetAccountKey(
                "Inventory");

        if (!PlayerPrefs.HasKey(key))
        {
            RefreshInventoryUI();
            return;
        }

        string json =
            PlayerPrefs.GetString(
                key,
                "");

        if (string.IsNullOrEmpty(json))
        {
            RefreshInventoryUI();
            return;
        }

        InventorySaveData saveData =
            JsonUtility.FromJson<InventorySaveData>(
                json);

        if (saveData == null ||
            saveData.items == null)
        {
            RefreshInventoryUI();
            return;
        }

        foreach (InventorySaveItem saveItem
                 in saveData.items)
        {
            ItemSO item =
                FindItemById(
                    saveItem.itemId);

            if (item == null)
            {
                Debug.LogWarning(
                    $"ItemSO를 찾을 수 없습니다. ID: {saveItem.itemId}");

                continue;
            }

            if (saveItem.count <= 0)
                continue;

            items.Add(
                new InventoryItem(
                    item,
                    saveItem.count));
        }

        RefreshInventoryUI();

        Debug.Log(
            $"인벤토리 불러오기 완료 : {key}");
    }


    ItemSO FindItemById(string itemId)
    {
        if (itemDatabase == null)
            return null;

        return itemDatabase.FindById(itemId);
    }


    void RefreshInventoryUI()
    {
        if (InventoryUI.Inst != null)
            InventoryUI.Inst.Refresh();
    }
}