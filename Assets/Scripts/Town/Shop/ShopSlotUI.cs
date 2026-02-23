using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image iconImage;
    [SerializeField] Button button;
    [SerializeField] TMP_Text lockText;
    [SerializeField] TMP_Text priceText;

    ShopItemSO item;
    System.Action<ShopItemSO, RectTransform> onClick;

    public RectTransform Rect => transform as RectTransform;

    public void SetLocked(bool locked, string lockedMsg = "?")
    {
        item = null;
        onClick = null;

        if (iconImage != null) iconImage.enabled = !locked;

        if (lockText != null)
        {
            lockText.gameObject.SetActive(locked);
            lockText.text = lockedMsg;
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

        if (lockText != null) lockText.gameObject.SetActive(false);

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