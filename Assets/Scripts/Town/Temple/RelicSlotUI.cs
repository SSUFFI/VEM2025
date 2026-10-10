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
    [SerializeField] GameObject relicChain;
    [SerializeField] GameObject relicLock;

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

        if (relicChain != null)
            relicChain.SetActive(!unlocked);

        if (relicLock != null)
            relicLock.SetActive(!unlocked);

        iconImage.gameObject.SetActive(true);

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

        RefreshBrightness(selected);
    }

    void RefreshBrightness(bool selected)
    {
        float brightness = selected ? 1f : 0.65f;

        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            Color color = image.color;
            color.r = brightness;
            color.g = brightness;
            color.b = brightness;
            image.color = color;
        }

        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
        {
            Color color = text.color;
            color.r = brightness;
            color.g = brightness;
            color.b = brightness;
            text.color = color;
        }
    }
}