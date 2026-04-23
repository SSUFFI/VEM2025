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

    ShopItemSO item;
    System.Action<ShopItemSO, RectTransform> onClick;

    public RectTransform Rect => transform as RectTransform;

    public void SetLocked(bool locked)
    {
        item = null;
        onClick = null;

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

    public void SetItem(ShopItemSO newItem, System.Action<ShopItemSO, RectTransform> clickCb)
    {
        item = newItem;
        onClick = clickCb;

        if (iconImage != null)
        {
            iconImage.enabled = (item != null && item.icon != null);
            iconImage.sprite = (item != null) ? item.icon : null;
        }

        if (priceText != null)
        {
            bool hasItem = (item != null);
            priceText.gameObject.SetActive(hasItem);
            priceText.text = hasItem ? $"{item.price:N0}원" : "";
        }

        if (button != null)
        {
            button.interactable = (item != null);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (item != null)
                    onClick?.Invoke(item, Rect);
            });
        }
    }
}