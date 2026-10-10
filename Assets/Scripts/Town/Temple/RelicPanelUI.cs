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
    [SerializeField] TMP_Text relicNameTMP;
    [SerializeField] TMP_Text descriptionTMP;
    [SerializeField] Image infoIcon;
    [SerializeField] TMP_Text manaCostTMP;

    [Header("Equip Button")]
    [SerializeField] Button equipButton;
    [SerializeField] TMP_Text equipButtonTMP;

    [Header("Enhance Button")]
    [SerializeField] Button enhanceButton;
    [SerializeField] TMP_Text enhanceButtonTMP;

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

        RefreshEnhanceButton();
    }

    void OnEnable()
    {
        RefreshEnhanceButton();
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

        relicNameTMP.text = data.relicName;

        descriptionTMP.text = selectedSlot.IsLocked()
            ? data.lockedDescription
            : data.description;

        infoIcon.sprite = data.icon;

        manaCostTMP.text = selectedSlot.IsLocked()
            ? "?"
            : data.manaCost.ToString();
    }

    public void OnClickEquip()
    {
        if (selectedSlot == null)
            return;

        RelicDataSO data = selectedSlot.Data;

        if (data == null || selectedSlot.IsLocked())
            return;

        if (PlayerRelicManager.Inst == null)
            return;

        PlayerRelicManager.Inst.EquipRelic(data);

        RefreshEquipButton();
    }

    void RefreshEquipButton()
    {
        if (selectedSlot == null)
            return;

        RelicDataSO data = selectedSlot.Data;

        if (data == null)
            return;

        if (selectedSlot.IsLocked())
        {
            equipButton.interactable = false;
            equipButtonTMP.text = "미해금";
            return;
        }

        bool equipped = PlayerRelicManager.Inst != null && PlayerRelicManager.Inst.equippedRelic == data;

        equipButton.interactable = !equipped;

        equipButtonTMP.text = equipped
            ? "장착 중"
            : "장착";
    }

    void RefreshEnhanceButton()
    {
        if (enhanceButton == null ||
            enhanceButtonTMP == null)
            return;

        string key = AccountManager.GetAccountKey("Stage1BossCleared");

        bool stage1Cleared = PlayerPrefs.GetInt(key, 0) == 1;

        enhanceButton.interactable = stage1Cleared;

        enhanceButtonTMP.text = stage1Cleared ? "강화" : "강화\n스테이지1 클리어 시 해금";
    }

    bool IsStage1BossCleared()
    {
        string key = AccountManager.GetAccountKey("Stage1BossCleared");
        return PlayerPrefs.GetInt(key, 0) == 1;
    }


}