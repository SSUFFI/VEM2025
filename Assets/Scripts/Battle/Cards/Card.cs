using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer Character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text manaTMP;
    [SerializeField] TMP_Text descriptionTMP;
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
            descriptionTMP.text = data.description;
        }
        else
        {
            card.sprite = cardBack;
            Character.sprite = null;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
            manaTMP.text = "";
            descriptionTMP.text = "";
        }
    }

    void OnMouseOver() { }
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
}