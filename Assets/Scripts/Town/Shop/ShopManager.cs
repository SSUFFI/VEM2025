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

    int openedSlotCount = 4;

    ShopSlotUI selectedSlot;

    void Awake()
    {
        Inst = this;

        if (shopPanel != null)
            shopPanel.SetActive(false);
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

    void RefreshDisplay()
    {
        var picks = PickUnique(itemPool, openedSlotCount);

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
                i < picks.Count ? picks[i] : null;

            if (item == null)
            {
                openSlots[i].SetLocked(true);
            }
            else
            {
                openSlots[i].SetItem(
                    item,
                    OnClickItem);
            }
        }
    }

    static List<ItemSO> PickUnique(
        List<ItemSO> pool,
        int count)
    {
        var result = new List<ItemSO>();

        if (pool == null ||
            pool.Count == 0 ||
            count <= 0)
            return result;

        var temp = new List<ItemSO>(pool);

        for (int i = 0; i < temp.Count; i++)
        {
            int r = Random.Range(i, temp.Count);

            (temp[i], temp[r]) =
                (temp[r], temp[i]);
        }

        int take = Mathf.Min(count, temp.Count);

        for (int i = 0; i < take; i++)
            result.Add(temp[i]);

        return result;
    }

    void OnClickItem(
        ItemSO item,
        RectTransform slotRect,
        ShopSlotUI slot)
    {
        if (popupUI == null || item == null)
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

        InventoryManager.Inst.AddItem(item, 1);

        selectedSlot.SetSoldOut();

        if (popupUI != null)
            popupUI.SetSoldOut();

        Debug.Log(
            $"{item.itemName} 구매 완료");
    }

    public void SetOpenedSlotCount(int count)
    {
        openedSlotCount =
            Mathf.Clamp(count, 0, openSlots.Count);

        if (shopPanel != null &&
            shopPanel.activeSelf)
        {
            RefreshDisplay();
        }
    }
}