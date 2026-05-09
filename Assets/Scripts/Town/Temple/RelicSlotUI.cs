using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image baseImage;
    [SerializeField] Image frameImage;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text manaTMP;
    [SerializeField] Button button;

    [Header("Sprites")]
    [SerializeField] Sprite lockSprite;

    [SerializeField] Sprite baseOpened;
    [SerializeField] Sprite baseSelected;

    [SerializeField] Sprite frameOpened;
    [SerializeField] Sprite frameSelected;

    RelicDataSO data;

    public RelicDataSO Data => data;

    public void Setup(RelicDataSO relicData)
    {
        data = relicData;

        iconImage.sprite = data.icon;
        manaTMP.text = data.manaCost.ToString();

        Refresh(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        RelicPanelUI.Inst.SelectRelic(this);
    }

    public void Refresh(bool selected)
    {
        if (data == null)
            return;

        bool unlocked = !data.isLocked;

        iconImage.gameObject.SetActive(unlocked);
        manaTMP.gameObject.SetActive(unlocked);

        if (data.isLocked)
        {
            baseImage.sprite = lockSprite;

            frameImage.gameObject.SetActive(false);
        }
        else
        {
            frameImage.gameObject.SetActive(true);

            if (selected)
            {
                baseImage.sprite = baseSelected;
                frameImage.sprite = frameSelected;
            }
            else
            {
                baseImage.sprite = baseOpened;
                frameImage.sprite = frameOpened;
            }
        }
    }
}