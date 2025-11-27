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
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    public Item item;
    public bool isMine;
    bool isFront;
    public PRS originPRS;


    public void Setup(Item item, bool isMine)
    {
        this.item = item;
        this.isMine = isMine;
        this.isFront = isMine;

        if (this.isFront)
        {
            card.sprite = cardFront;
            Character.sprite = this.item.sprite;
            nameTMP.text = this.item.name;
            attackTMP.text = this.item.attack.ToString();
            healthTMP.text = this.item.health.ToString();
        }
        else
        {
            card.sprite = cardBack;
            Character.sprite = null;
            nameTMP.text = string.Empty;
            attackTMP.text = string.Empty;
            healthTMP.text = string.Empty;
        }
    }

    void OnMouseOver() { }
    void OnMouseExit() { }

    void OnMouseDown()
    {
        if (!isMine) return;

        CardManager.Inst.OnCardClicked(this);
    }

    void OnMouseUp()
    {

        if (isMine)
            CardManager.Inst.CardMouseUp();
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
