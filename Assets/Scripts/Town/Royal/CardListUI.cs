
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
    public Image raceImage;

    public TMP_Text nameTMP;
    public TMP_Text attackTMP;
    public TMP_Text healthTMP;
    public TMP_Text manaTMP;

    [Header("Descriptions")]
    public TMP_Text descriptionTopTMP;
    public TMP_Text descriptionMiddleTMP;
    public TMP_Text descriptionBottomTMP;
    public TMP_Text descriptionFloorTMP;

    [Header("Description Gems")]
    public Image topGem;
    public Image middleGem;
    public Image bottomGem;

    bool isHolding;
    bool isMaxCount;

    Color nameOriginColor;
    Color attackOriginColor;
    Color healthOriginColor;
    Color manaOriginColor;
    Color raceOriginColor;

    Color topOriginColor;
    Color middleOriginColor;
    Color bottomOriginColor;
    Color floorOriginColor;

    Color topGemOriginColor;
    Color middleGemOriginColor;
    Color bottomGemOriginColor;

    void Awake()
    {
        nameOriginColor = nameTMP.color;
        attackOriginColor = attackTMP.color;
        healthOriginColor = healthTMP.color;
        manaOriginColor = manaTMP.color;

        if (raceImage != null)
            raceOriginColor = raceImage.color;

        if (descriptionTopTMP != null)
            topOriginColor = descriptionTopTMP.color;

        if (descriptionMiddleTMP != null)
            middleOriginColor = descriptionMiddleTMP.color;

        if (descriptionBottomTMP != null)
            bottomOriginColor = descriptionBottomTMP.color;

        if (descriptionFloorTMP != null)
            floorOriginColor = descriptionFloorTMP.color;

        if (topGem != null)
            topGemOriginColor = topGem.color;

        if (middleGem != null)
            middleGemOriginColor = middleGem.color;

        if (bottomGem != null)
            bottomGemOriginColor = bottomGem.color;
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

        if (raceImage != null)
            raceImage.color = isMaxCount ? gray : raceOriginColor;

        nameTMP.color = isMaxCount ? gray : nameOriginColor;
        attackTMP.color = isMaxCount ? gray : attackOriginColor;
        healthTMP.color = isMaxCount ? gray : healthOriginColor;
        manaTMP.color = isMaxCount ? gray : manaOriginColor;

        if (descriptionTopTMP != null)
            descriptionTopTMP.color =
                isMaxCount ? gray : topOriginColor;

        if (descriptionMiddleTMP != null)
            descriptionMiddleTMP.color =
                isMaxCount ? gray : middleOriginColor;

        if (descriptionBottomTMP != null)
            descriptionBottomTMP.color =
                isMaxCount ? gray : bottomOriginColor;

        if (descriptionFloorTMP != null)
            descriptionFloorTMP.color =
                isMaxCount ? gray : floorOriginColor;

        if (topGem != null)
            topGem.color = isMaxCount ? gray : topGemOriginColor;

        if (middleGem != null)
            middleGem.color = isMaxCount ? gray : middleGemOriginColor;

        if (bottomGem != null)
            bottomGem.color = isMaxCount ? gray : bottomGemOriginColor;
    }

    public void Init(CardDataSO data)
    {
        this.data = data;

        nameTMP.text = data.cardName;
        cardImage.sprite = data.sprite;

        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();
        manaTMP.text = data.manaCost.ToString();

        SetupDescriptions(data);

        if (raceImage != null)
        {
            raceImage.sprite = data.raceSprite;
            raceImage.gameObject.SetActive(data.raceSprite != null);
        }

        RefreshState();
    }

    void SetupDescriptions(CardDataSO data)
    {
        SetDescription(
            descriptionTopTMP,
            topGem,
            data.descriptionTop
        );

        SetDescription(
            descriptionMiddleTMP,
            middleGem,
            data.descriptionMiddle
        );

        SetDescription(
            descriptionBottomTMP,
            bottomGem,
            data.descriptionBottom
        );

        SetDescription(
            descriptionFloorTMP,
            null,
            data.descriptionFloor
        );
    }

    void SetDescription(
        TMP_Text text,
        Image gem,
        string description)
    {
        bool hasText = !string.IsNullOrWhiteSpace(description);

        if (text != null)
        {
            text.text = hasText ? description : "";
            text.gameObject.SetActive(hasText);
        }

        if (gem != null)
            gem.gameObject.SetActive(hasText);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (data == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CardPreviewManager.Inst != null)
                CardPreviewManager.Inst.Show(data);

            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (DeckEditManager.Inst == null)
                return;

            if (isMaxCount)
                return;

            if (!isHolding)
                DeckListUI.Inst.StartAdd(data);
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
