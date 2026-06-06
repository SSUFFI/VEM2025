using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text countTMP;

    ItemSO item;

    public void Setup(InventoryItem invenItem)
    {
        if (invenItem == null || invenItem.item == null)
            return;

        item = invenItem.item;

        if (itemIcon != null)
            itemIcon.sprite = item.icon;

        if (countTMP != null)
        {
            countTMP.gameObject.SetActive(invenItem.count > 1);
            countTMP.text = $"x{invenItem.count}";
        }
    }
}