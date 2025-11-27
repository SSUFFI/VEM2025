using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] Item item;
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;   // ★ 다시 복구됨

    public int attack;
    public int health;
    public bool isMine;
    public bool isDie;
    public bool isBossOrEmpty;  // 보스 or 빈 슬롯
    public bool attackable;
    public Vector3 originPos;
    int liveCount;

    public Item ItemData => item;

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;

        // ★ 보스면 체력 TMP 숨김 처리
        if (isBossOrEmpty && healthTMP != null)
            healthTMP.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (isBossOrEmpty)
            return;

        if (isMine == myTurn)
            liveCount++;
    }

    public void Setup(Item item)
    {
        attack = item.attack;
        health = item.health;

        this.item = item;
        character.sprite = this.item.sprite;

        nameTMP.text = this.item.name;
        attackTMP.text = attack.ToString();

        // ★ 보스는 체력 표시 X, 일반 엔티티는 표시
        if (!isBossOrEmpty)
            healthTMP.text = health.ToString();
        else
            healthTMP.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
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

    // ★ 보스는 체력 시스템 없음 → 데미지 없음
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
