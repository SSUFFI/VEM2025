using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] CardData data;          // ✅ Item -> CardData
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;

    public int attack;
    public int health;
    public bool isMine;
    public bool isDie;
    public bool isBossOrEmpty;  // 보스 or 빈 슬롯
    public bool attackable;
    public Vector3 originPos;
    int liveCount;

    public CardData Data => data;            // ✅ ItemData -> Data

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;

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

    public void Setup(CardData data)         // ✅ Item -> CardData
    {
        attack = data.attack;
        health = data.health;

        this.data = data;
        character.sprite = this.data.sprite;

        nameTMP.text = this.data.name;
        attackTMP.text = attack.ToString();

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