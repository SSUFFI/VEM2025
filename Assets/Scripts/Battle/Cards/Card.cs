
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer Character;
    [SerializeField] SpriteRenderer raceRenderer;

    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text manaTMP;

    [Header("Descriptions")]
    [SerializeField] TMP_Text descriptionTopTMP;
    [SerializeField] TMP_Text descriptionMiddleTMP;
    [SerializeField] TMP_Text descriptionBottomTMP;
    [SerializeField] TMP_Text descriptionFloorTMP;

    [Header("Description Gems")]
    [SerializeField] GameObject topGem;
    [SerializeField] GameObject middleGem;
    [SerializeField] GameObject bottomGem;

    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    public SpriteRenderer CardFrameRenderer => card;

    public CardDataSO dataSO;

    public bool isMine;
    bool isFront;
    public PRS originPRS;
    public PRS zoomPRS;
    public bool hasZoomPRS;
    public bool isField;

    public void Setup(CardDataSO data, bool isMine, bool isField = false)
    {
        this.dataSO = data;
        this.isMine = isMine;
        this.isField = isField;
        this.isFront = isMine;

        if (this.isFront)
        {
            card.sprite = cardFront;

            Character.sprite = isField && data.fieldSprite != null
                ? data.fieldSprite
                : data.sprite;

            nameTMP.text = data.cardName;
            attackTMP.text = data.attack.ToString();
            healthTMP.text = data.health.ToString();
            manaTMP.text = data.manaCost.ToString();

            SetupDescriptions(data);

            if (raceRenderer != null)
            {
                raceRenderer.sprite = data.raceSprite;
                raceRenderer.gameObject.SetActive(data.raceSprite != null);
            }
        }
        else
        {
            card.sprite = cardBack;
            Character.sprite = null;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
            manaTMP.text = "";

            HideDescriptions();

            if (raceRenderer != null)
            {
                raceRenderer.sprite = null;
                raceRenderer.gameObject.SetActive(false);
            }
        }
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

    void SetDescription(TMP_Text text, GameObject gem, string description)
    {
        bool hasText = !string.IsNullOrWhiteSpace(description);

        if (text != null)
        {
            text.text = hasText ? description : "";
            text.gameObject.SetActive(hasText);
        }

        if (gem != null)
            gem.SetActive(hasText);
    }

    void HideDescriptions()
    {
        SetDescription(descriptionTopTMP, topGem, "");
        SetDescription(descriptionMiddleTMP, middleGem, "");
        SetDescription(descriptionBottomTMP, bottomGem, "");
        SetDescription(descriptionFloorTMP, null, "");
    }

    void OnMouseExit() { }
    void OnMouseUp() { }

    void OnMouseDown()
    {
        if (!isMine) return;

        CardManager.Inst.OnCardClicked(this);
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }

    void OnMouseOver()
    {
        if (CardManager.Inst != null &&
            !CardManager.Inst.IsZoomMode)
            return;

        if (CardPreviewManager.Inst != null &&
            CardPreviewManager.Inst.IsOpen)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (dataSO == null)
                return;

            if (CardPreviewManager.Inst == null)
                return;

            CardPreviewManager.Inst.Show(dataSO);
        }
    }
}
