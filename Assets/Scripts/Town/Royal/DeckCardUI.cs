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

    public void Init(CardDataSO data, int count)
    {
        this.data = data;
        nameTMP.text = $"{data.cardName} x{count}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeckListUI.Inst.StartRemove(data);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        DeckListUI.Inst.StartHoldRemove(data);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        DeckListUI.Inst.StopHold();
    }
}