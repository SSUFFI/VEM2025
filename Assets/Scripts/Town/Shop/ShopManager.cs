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
    [SerializeField] List<ShopItemSO> itemPool;

    [Header("Popup")]
    [SerializeField] ShopPopupUI popupUI;

    int openedSlotCount = 4;

    void Awake()
    {
        Inst = this;
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);

        if (popupUI != null) popupUI.Hide();

        if (lockedSlots != null)
        {
            for (int i = 0; i < lockedSlots.Count; i++)
            {
                if (lockedSlots[i] != null)
                    lockedSlots[i].SetLocked(true, "?");
            }
        }

        RefreshDisplay();
    }

    public void CloseShop()
    {
        if (popupUI != null) popupUI.Hide();
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void RefreshDisplay()
    {
        var picks = PickUnique(itemPool, openedSlotCount);

        for (int i = 0; i < openSlots.Count; i++)
        {
            if (openSlots[i] == null) continue;

            if (i >= openedSlotCount)
            {
                openSlots[i].SetLocked(true, "?");
                continue;
            }

            var item = (i < picks.Count) ? picks[i] : null;

            if (item == null)
                openSlots[i].SetLocked(true, "¹Ìµî·Ï");
            else
                openSlots[i].SetItem(item, OnClickItem);
        }
    }

    static List<ShopItemSO> PickUnique(List<ShopItemSO> pool, int count)
    {
        var result = new List<ShopItemSO>();
        if (pool == null || pool.Count == 0 || count <= 0) return result;

        var temp = new List<ShopItemSO>(pool);

        for (int i = 0; i < temp.Count; i++)
        {
            int r = Random.Range(i, temp.Count);
            (temp[i], temp[r]) = (temp[r], temp[i]);
        }

        int take = Mathf.Min(count, temp.Count);
        for (int i = 0; i < take; i++)
            result.Add(temp[i]);

        return result;
    }

    void OnClickItem(ShopItemSO item, RectTransform slotRect)
    {
        if (popupUI == null || item == null) return;

        if (popupUI.IsOpen)
        {
            popupUI.Hide();
            return;
        }

        popupUI.Show(item, slotRect, OnBuyItem);
    }

    void OnBuyItem(ShopItemSO item)
    {
        if (popupUI != null) popupUI.Hide();
    }

    public void SetOpenedSlotCount(int count)
    {
        openedSlotCount = Mathf.Clamp(count, 0, openSlots.Count);
        if (shopPanel != null && shopPanel.activeSelf)
            RefreshDisplay();
    }
}