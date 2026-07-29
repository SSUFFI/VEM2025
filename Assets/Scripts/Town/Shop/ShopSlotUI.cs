using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image iconImage;
    [SerializeField] Button button;
    [SerializeField] TMP_Text priceText;

    [Header("Lock Icon")]
    [SerializeField] Sprite lockIcon;

    ItemSO item;
    System.Action<ItemSO, RectTransform, ShopSlotUI> onClick;

    bool isSoldOut;

    public RectTransform Rect => transform as RectTransform;
    public bool IsSoldOut => isSoldOut;

    public void SetLocked(bool locked)
    {
        item = null;
        onClick = null;
        isSoldOut = false;

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = locked ? lockIcon : null;
        }

        if (priceText != null)
            priceText.gameObject.SetActive(false);

        if (button != null)
        {
            button.interactable = !locked;
            button.onClick.RemoveAllListeners();
        }
    }

    public void SetItem(
        ItemSO newItem,
        System.Action<ItemSO, RectTransform, ShopSlotUI> clickCb)
    {
        item = newItem;
        onClick = clickCb;
        isSoldOut = false;

        if (iconImage != null)
        {
            iconImage.enabled = item != null && item.icon != null;
            iconImage.sprite = item != null ? item.icon : null;
        }

        if (priceText != null)
        {
            bool hasItem = item != null;

            priceText.gameObject.SetActive(hasItem);
            priceText.text = hasItem ? $"{item.price:N0}원" : "";
        }

        if (button != null)
        {
            button.interactable = item != null;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (item == null || isSoldOut)
                    return;

                onClick?.Invoke(item, Rect, this);
            });
        }
    }

    public void SetSoldOut()
    {
        isSoldOut = true;

        if (priceText != null)
        {
            priceText.gameObject.SetActive(true);
            priceText.text = "품절";
        }

        if (button != null)
        {
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
    }
}