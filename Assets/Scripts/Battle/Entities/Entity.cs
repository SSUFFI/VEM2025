using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] CardDataSO dataSO;
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer character;
    [SerializeField] SpriteRenderer frameRenderer;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;

    public int attack;
    public int health;
    public bool isMine;
    public bool isDie;
    public bool isBossOrEmpty;
    public bool attackable;
    public bool addedToGrave = false;
    public Vector3 originPos;
    int liveCount;

    Tween tauntTween;
    Color tauntBlue = new Color(0.55f, 0.85f, 1f);

    public CardDataSO Data => dataSO;

    public bool HasTaunt =>
        dataSO != null &&
        dataSO.taunt;

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;

        if (isBossOrEmpty && healthTMP != null)
            healthTMP.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;

        tauntTween?.Kill();
    }

    void OnTurnStarted(bool myTurn)
    {
        if (isBossOrEmpty)
            return;

        if (isMine == myTurn)
            liveCount++;
    }

    public void Setup(CardDataSO data)
    {
        this.dataSO = data;

        attack = data.attack;
        health = data.health;

        character.sprite = data.fieldSprite != null
            ? data.fieldSprite
            : data.sprite;

        nameTMP.text = data.cardName;
        attackTMP.text = attack.ToString();

        if (!isBossOrEmpty)
            healthTMP.text = health.ToString();
        else
            healthTMP.gameObject.SetActive(false);

        SetAttackable(false);
    }

    public void SetupBoss(Sprite portrait)
    {
        if (character != null && portrait != null)
            character.sprite = portrait;

        SetAttackable(false);
    }

    public void SetAttackable(bool value)
    {
        attackable = value;

        Color baseColor = value
            ? Color.white
            : new Color(120f / 255f, 120f / 255f, 120f / 255f);

        if (frameRenderer == null)
            return;

        tauntTween?.Kill();

        if (HasTaunt)
        {
            frameRenderer.color = baseColor;

            Color pulseColor = value
                ? new Color(1f, 0.35f, 0.35f)
                : new Color(0.45f, 0.20f, 0.20f);

            tauntTween = frameRenderer.DOColor(pulseColor, 1.2f)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            frameRenderer.color = baseColor;
        }
    }

    void OnMouseOver()
    {
        if (CardPreviewManager.Inst != null && CardPreviewManager.Inst.IsOpen)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (dataSO == null) return;
            if (CardPreviewManager.Inst == null) return;

            CardPreviewManager.Inst.Show(dataSO);
        }
    }

    void OnMouseDown()
    {
        if (CardPreviewManager.Inst != null && CardPreviewManager.Inst.IsOpen)
            return;

        if (isMine)
            EntityManager.Inst.EntityMouseDown(this);
    }

    void OnMouseUp()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseUp();
    }

    void OnMouseDrag()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDrag();
    }

    public bool Damaged(int damage)
    {
        if (isBossOrEmpty)
            return false;

        health -= damage;
        healthTMP.text = health.ToString();

        if (health <= 0)
        {
            isDie = true;
            return true;
        }

        return false;
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
            transform.DOMove(pos, dotweenTime);
        else
            transform.position = pos;
    }
}