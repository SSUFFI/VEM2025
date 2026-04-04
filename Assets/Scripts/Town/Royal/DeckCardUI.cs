using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardUI : MonoBehaviour, IPointerClickHandler
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
            DeckEditManager.Inst.RemoveCard(data);

            DeckListUI.Inst.Refresh();
        }
    }
}