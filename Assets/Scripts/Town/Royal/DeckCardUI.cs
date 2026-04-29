using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public TMP_Text nameTMP;

    CardDataSO data;
    bool isHolding;

    public void Init(CardDataSO data, int count)
    {
        this.data = data;
        nameTMP.text = $"{data.cardName} x{count}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isHolding)
                DeckListUI.Inst.StartRemove(data);
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
        DeckListUI.Inst.StartHoldRemove(data);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        CancelInvoke(nameof(StartHold));
        DeckListUI.Inst.StopHold();
    }
}