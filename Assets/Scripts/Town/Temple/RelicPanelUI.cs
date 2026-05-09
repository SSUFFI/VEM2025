using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RelicPanelUI : MonoBehaviour
{
    public static RelicPanelUI Inst;

    void Awake() => Inst = this;

    [Header("Relics")]
    [SerializeField] List<RelicSlotUI> relicSlots;

    [SerializeField] List<RelicDataSO> relicDatas;

    [Header("Info Panel")]
    [SerializeField] TMP_Text descriptionTMP;
    [SerializeField] Image infoIcon;
    [SerializeField] TMP_Text manaCostTMP;

    [Header("Equip Button")]
    [SerializeField] Button equipButton;
    [SerializeField] TMP_Text equipButtonTMP;

    RelicSlotUI selectedSlot;

    void Start()
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (i >= relicDatas.Count)
                break;

            relicSlots[i].Setup(relicDatas[i]);
        }

        if (relicSlots.Count > 0)
            SelectRelic(relicSlots[0]);

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnClickEquip);
        }
    }

    public void SelectRelic(RelicSlotUI slot)
    {
        selectedSlot = slot;

        foreach (var s in relicSlots)
        {
            bool selected = s == selectedSlot;
            s.Refresh(selected);
        }

        RefreshInfo();
        RefreshEquipButton();
    }

    void RefreshInfo()
    {
        if (selectedSlot == null)
            return;

        RelicDataSO data = selectedSlot.Data;

        if (data == null)
            return;

        descriptionTMP.text = data.description;
        infoIcon.sprite = data.icon;
        manaCostTMP.text = data.manaCost.ToString();
    }

    public void OnClickEquip()
    {
        if (selectedSlot == null)
            return;

        if (PlayerRelicManager.Inst == null)
            return;

        PlayerRelicManager.Inst.EquipRelic(selectedSlot.Data);

        RefreshEquipButton();
    }

    void RefreshEquipButton()
    {
        if (selectedSlot == null)
            return;

        bool equipped =
            PlayerRelicManager.Inst != null &&
            PlayerRelicManager.Inst.equippedRelic == selectedSlot.Data;

        equipButton.interactable = !equipped;

        equipButtonTMP.text = equipped
            ? "ÀåÂø Áß"
            : "ÀåÂø";
    }
}