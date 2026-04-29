using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardListUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public CardDataSO data;

    public Image cardImage;
    public TMP_Text nameTMP;
    public TMP_Text attackTMP;
    public TMP_Text healthTMP;
    public TMP_Text manaTMP;
    public TMP_Text descriptionTMP;

    bool isHolding;

    public void Init(CardDataSO data)
    {
        this.data = data;

        nameTMP.text = data.cardName;
        cardImage.sprite = data.sprite;

        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();
        manaTMP.text = data.manaCost.ToString();
        descriptionTMP.text = data.description;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DeckEditManager.Inst == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isHolding)
                DeckListUI.Inst.StartAdd(data);
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CardPreviewManager.Inst != null)
                CardPreviewManager.Inst.Show(data);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isHolding = false;
        Invoke(nameof(StartHold), 0.18f);
    }

    void StartHold()
    {
        isHolding = true;
        DeckListUI.Inst.StartHoldAdd(data);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        CancelInvoke(nameof(StartHold));
    }
}