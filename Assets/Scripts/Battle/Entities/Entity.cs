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

    [Header("Relic")]
    [SerializeField] GameObject relicTargetMark;

    [Header("Taunt")]
    [SerializeField] GameObject tauntObject;

    public int attack;
    public int health;
    public int maxHealth;

    public bool isMine;
    public bool isDie;
    public bool isBossOrEmpty;
    public bool attackable;
    public bool addedToGrave = false;
    public Vector3 originPos;
    int liveCount;

    Tween relicMarkTween;

    public CardDataSO Data => dataSO;

    public bool HasTaunt =>
        dataSO != null &&
        dataSO.taunt;

    public bool CanAttack()
    {
        if (isDie) return false;
        if (isBossOrEmpty) return false;
        if (attack <= 0) return false;

        return attackable;
    }

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;

        if (isBossOrEmpty && healthTMP != null)
            healthTMP.gameObject.SetActive(false);

        SetRelicTargetMark(false);
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;

        relicMarkTween?.Kill();
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
        maxHealth = data.health;

        character.sprite = data.fieldSprite != null
            ? data.fieldSprite
            : data.sprite;

        nameTMP.text = data.cardName;
        attackTMP.text = attack.ToString();

        if (!isBossOrEmpty)
            healthTMP.text = health.ToString();
        else
            healthTMP.gameObject.SetActive(false);

        if (tauntObject != null)
            tauntObject.SetActive(data.taunt);

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

        frameRenderer.color = baseColor;
    }

    public void SetRelicTargetMark(bool value)
    {
        if (relicTargetMark == null)
            return;

        relicMarkTween?.Kill();

        Vector3 baseScale = Vector3.one * 19f;

        relicTargetMark.transform.localScale =
            baseScale;

        if (!value)
        {
            relicTargetMark.SetActive(false);
            return;
        }

        relicTargetMark.SetActive(true);

        relicMarkTween =
            relicTargetMark.transform
            .DOScale(baseScale * 1.15f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    public void UpdateHealthUI()
    {
        if (healthTMP != null)
            healthTMP.text = health.ToString();
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
        if (BattleRelicUI.Inst != null &&
            BattleRelicUI.Inst.IsTargeting)
            return;

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
        if (BattleRelicUI.Inst != null &&
            BattleRelicUI.Inst.IsTargeting)
            return;

        if (isMine)
            EntityManager.Inst.EntityMouseDrag();
    }

    void OnMouseUpAsButton()
    {
        if (BattleRelicUI.Inst == null)
            return;

        if (!BattleRelicUI.Inst.IsTargeting)
            return;

        BattleRelicUI.Inst.OnEntitySelected(this);
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

            Collider2D[] cols = GetComponentsInChildren<Collider2D>();

            foreach (var col in cols)
                col.enabled = false;

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