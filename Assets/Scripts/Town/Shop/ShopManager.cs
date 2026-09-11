using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Inst { get; private set; }

    [Header("Panel")]
    [SerializeField] GameObject shopPanel;

    [Header("Slots (Top Open 4, Bottom Locked 8)")]
    [SerializeField] List<ShopSlotUI> openSlots;
    [SerializeField] List<ShopSlotUI> lockedSlots;

    [Header("Item Pool")]
    [SerializeField] List<ItemSO> itemPool;

    [Header("Currency")]
    [SerializeField] ItemSO goldItem;

    [Header("Popup")]
    [SerializeField] ShopPopupUI popupUI;

    [Header("Refresh")]
    [SerializeField] float refreshHours = 6f;

    int openedSlotCount = 4;

    ShopSlotUI selectedSlot;

    List<ItemSO> currentShopItems = new List<ItemSO>();
    List<bool> soldOutStates = new List<bool>();

    string GetLastRefreshKey()
    {
        return AccountManager.GetAccountKey("Shop_LastRefresh");
    }

    string GetItemIndexKey(int index)
    {
        return AccountManager.GetAccountKey($"Shop_Item_{index}");
    }

    string GetSoldOutKey(int index)
    {
        return AccountManager.GetAccountKey($"Shop_SoldOut_{index}");
    }

    void Awake()
    {
        Inst = this;

        if (shopPanel != null)
            shopPanel.SetActive(false);

        LoadOrCreateShop();
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

        selectedSlot = null;

        if (popupUI != null)
            popupUI.Hide();

        if (lockedSlots != null)
        {
            for (int i = 0; i < lockedSlots.Count; i++)
            {
                if (lockedSlots[i] != null)
                    lockedSlots[i].SetLocked(true);
            }
        }

        CheckRefresh();
        RefreshDisplay();
    }

    public void CloseShop()
    {
        selectedSlot = null;

        if (popupUI != null)
            popupUI.Hide();

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    void LoadOrCreateShop()
    {
        string refreshKey = GetLastRefreshKey();

        if (!PlayerPrefs.HasKey(refreshKey))
        {
            GenerateNewShop();
            return;
        }

        if (IsRefreshTime())
        {
            GenerateNewShop();
            return;
        }

        LoadShop();
    }

    void CheckRefresh()
    {
        if (IsRefreshTime())
            GenerateNewShop();
    }

    bool IsRefreshTime()
    {
        string refreshKey = GetLastRefreshKey();

        if (!PlayerPrefs.HasKey(refreshKey))
            return true;

        string saved = PlayerPrefs.GetString(refreshKey);

        if (!long.TryParse(saved, out long ticks))
            return true;

        DateTime lastRefresh =
            new DateTime(ticks, DateTimeKind.Utc);

        TimeSpan elapsed =
            DateTime.UtcNow - lastRefresh;

        return elapsed.TotalHours >= refreshHours;
    }

    void GenerateNewShop()
    {
        currentShopItems.Clear();
        soldOutStates.Clear();

        var picks =
            PickUnique(itemPool, openedSlotCount);

        for (int i = 0; i < openedSlotCount; i++)
        {
            ItemSO item =
                i < picks.Count ? picks[i] : null;

            currentShopItems.Add(item);
            soldOutStates.Add(false);
        }

        SaveShop();

        PlayerPrefs.SetString(
            GetLastRefreshKey(),
            DateTime.UtcNow.Ticks.ToString());

        PlayerPrefs.Save();

        Debug.Log("상점 상품이 새로 갱신되었습니다.");
    }

    void SaveShop()
    {
        for (int i = 0; i < openedSlotCount; i++)
        {
            ItemSO item =
                i < currentShopItems.Count
                    ? currentShopItems[i]
                    : null;

            int itemIndex = -1;

            if (item != null)
                itemIndex = itemPool.IndexOf(item);

            PlayerPrefs.SetInt(
                GetItemIndexKey(i),
                itemIndex);

            bool soldOut =
                i < soldOutStates.Count &&
                soldOutStates[i];

            PlayerPrefs.SetInt(
                GetSoldOutKey(i),
                soldOut ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    void LoadShop()
    {
        currentShopItems.Clear();
        soldOutStates.Clear();

        for (int i = 0; i < openedSlotCount; i++)
        {
            int index =
                PlayerPrefs.GetInt(
                    GetItemIndexKey(i),
                    -1);

            ItemSO item = null;

            if (index >= 0 &&
                index < itemPool.Count)
            {
                item = itemPool[index];
            }

            currentShopItems.Add(item);

            bool soldOut =
                PlayerPrefs.GetInt(
                    GetSoldOutKey(i),
                    0) == 1;

            soldOutStates.Add(soldOut);
        }
    }

    void RefreshDisplay()
    {
        for (int i = 0; i < openSlots.Count; i++)
        {
            if (openSlots[i] == null)
                continue;

            if (i >= openedSlotCount)
            {
                openSlots[i].SetLocked(true);
                continue;
            }

            ItemSO item =
                i < currentShopItems.Count
                    ? currentShopItems[i]
                    : null;

            if (item == null)
            {
                openSlots[i].SetLocked(true);
                continue;
            }

            openSlots[i].SetItem(
                item,
                OnClickItem);

            if (i < soldOutStates.Count &&
                soldOutStates[i])
            {
                openSlots[i].SetSoldOut();
            }
        }
    }

    static List<ItemSO> PickUnique(
        List<ItemSO> pool,
        int count)
    {
        var result =
            new List<ItemSO>();

        if (pool == null ||
            pool.Count == 0 ||
            count <= 0)
            return result;

        var temp =
            new List<ItemSO>(pool);

        for (int i = 0; i < temp.Count; i++)
        {
            int r =
                UnityEngine.Random.Range(
                    i,
                    temp.Count);

            (temp[i], temp[r]) =
                (temp[r], temp[i]);
        }

        int take =
            Mathf.Min(
                count,
                temp.Count);

        for (int i = 0; i < take; i++)
            result.Add(temp[i]);

        return result;
    }

    void OnClickItem(
        ItemSO item,
        RectTransform slotRect,
        ShopSlotUI slot)
    {
        if (popupUI == null ||
            item == null)
            return;

        if (popupUI.IsOpen)
        {
            popupUI.Hide();
            selectedSlot = null;
            return;
        }

        selectedSlot = slot;

        popupUI.Show(
            item,
            slotRect,
            OnBuyItem);
    }

    void OnBuyItem(ItemSO item)
    {
        if (item == null)
            return;

        if (selectedSlot == null ||
            selectedSlot.IsSoldOut)
            return;

        if (InventoryManager.Inst == null)
        {
            Debug.LogWarning(
                "InventoryManager가 없습니다.");
            return;
        }

        if (goldItem == null)
        {
            Debug.LogWarning(
                "ShopManager의 Gold Item이 연결되지 않았습니다.");
            return;
        }

        bool paid =
            InventoryManager.Inst.TryRemoveItem(
                goldItem,
                item.price);

        if (!paid)
        {
            Debug.Log("금화가 부족합니다.");
            return;
        }

        InventoryManager.Inst.AddItem(
            item,
            1);

        int slotIndex =
            openSlots.IndexOf(selectedSlot);

        if (slotIndex >= 0 &&
            slotIndex < soldOutStates.Count)
        {
            soldOutStates[slotIndex] = true;
            SaveShop();
        }

        selectedSlot.SetSoldOut();

        if (popupUI != null)
            popupUI.SetSoldOut();

        Debug.Log(
            $"{item.itemName} 구매 완료");
    }

    public void SetOpenedSlotCount(int count)
    {
        openedSlotCount =
            Mathf.Clamp(
                count,
                0,
                openSlots.Count);

        if (shopPanel != null &&
            shopPanel.activeSelf)
        {
            RefreshDisplay();
        }
    }
}