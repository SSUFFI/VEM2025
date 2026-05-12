using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    bool isMaxCount;

    Color nameOriginColor;
    Color attackOriginColor;
    Color healthOriginColor;
    Color manaOriginColor;
    Color descriptionOriginColor;

    void Awake()
    {
        nameOriginColor = nameTMP.color;
        attackOriginColor = attackTMP.color;
        healthOriginColor = healthTMP.color;
        manaOriginColor = manaTMP.color;
        descriptionOriginColor = descriptionTMP.color;
    }

    void Update()
    {
        RefreshState();
    }

    void RefreshState()
    {
        if (DeckEditManager.Inst == null || data == null)
            return;

        int count = DeckEditManager.Inst.currentDeck.Count(x => x == data);

        isMaxCount = count >= 4;

        Color gray = new Color(0.45f, 0.45f, 0.45f);

        cardImage.color = isMaxCount ? gray : Color.white;

        nameTMP.color = isMaxCount ? gray : nameOriginColor;
        attackTMP.color = isMaxCount ? gray : attackOriginColor;
        healthTMP.color = isMaxCount ? gray : healthOriginColor;
        manaTMP.color = isMaxCount ? gray : manaOriginColor;
        descriptionTMP.color = isMaxCount ? gray : descriptionOriginColor;
    }

    public void Init(CardDataSO data)
    {
        this.data = data;

        nameTMP.text = data.cardName;
        cardImage.sprite = data.sprite;

        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();
        manaTMP.text = data.manaCost.ToString();
        descriptionTMP.text = data.description;

        RefreshState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DeckEditManager.Inst == null) return;

        if (isMaxCount)
            return;

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
        if (isMaxCount)
            return;

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