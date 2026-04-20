using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardListUI : MonoBehaviour, IPointerClickHandler
{
    public CardDataSO data;

    public Image cardImage;
    public TMP_Text nameTMP;
    public TMP_Text attackTMP;
    public TMP_Text healthTMP;
    public TMP_Text manaTMP;
    public TMP_Text descriptionTMP;

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

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (DeckEditManager.Inst.AddCard(data))
            {
                if (DeckListUI.Inst != null)
                    DeckListUI.Inst.Refresh();
            }
        }

        // 좌클릭 → (나중에 확대)
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"카드 확대: {data.cardName}");
        }
    }
}