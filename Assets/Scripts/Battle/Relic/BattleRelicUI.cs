using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class BattleRelicUI : MonoBehaviour
{
    public static BattleRelicUI Inst;

    void Awake() => Inst = this;

    [SerializeField] SpriteRenderer relicRenderer;

    [Header("Mana UI")]
    [SerializeField] GameObject manaCostRoot;
    [SerializeField] TextMeshPro manaCostTMP;

    public GameObject ManaCostRoot => manaCostRoot;

    [Header("Target Guide")]
    [SerializeField] GameObject targetGuideRoot;
    [SerializeField] TextMeshPro targetGuideTMP;

    bool usedThisTurn;
    bool targeting;

    Coroutine guideHideCo;

    Color enabledColor = Color.white;

    Color disabledColor =
        new Color(0.4f, 0.4f, 0.4f);

    public bool IsTargeting => targeting;

    void Start()
    {
        SetupRelic();

        if (targetGuideRoot != null)
            targetGuideRoot.SetActive(false);

        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }

        RefreshVisual();
    }

    void CheckClick()
    {
        if (CardManager.Inst != null &&
            CardManager.Inst.IsZoomMode)
            return;

        Vector2 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits =
            Physics2D.OverlapPointAll(mousePos);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                OnRelicClicked();
                break;
            }
        }
    }

    void SetupRelic()
    {
        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        if (relicRenderer != null)
            relicRenderer.sprite = relic.battleButtonSprite;

        if (manaCostTMP != null)
            manaCostTMP.text = relic.manaCost.ToString();

        RefreshVisual();
    }

    void RefreshVisual()
    {
        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        bool canUse =
            TurnManager.Inst.myTurn &&
            !usedThisTurn &&
            TurnManager.Inst.CanPayMana(true, relic.manaCost);

        if (canUse &&
            relic.effectType == RelicEffectType.HealAlly2)
        {
            bool existHealTarget = false;

            foreach (var e in EntityManager.Inst.MyEntities)
            {
                if (e == null) continue;
                if (e.isDie) continue;
                if (e.isBossOrEmpty) continue;

                if (e.health < e.maxHealth)
                {
                    existHealTarget = true;
                    break;
                }
            }

            canUse = existHealTarget;
        }

        Color targetColor =
            canUse ? enabledColor : disabledColor;

        if (relicRenderer != null)
            relicRenderer.color = targetColor;
    }

    void OnTurnStarted(bool myTurn)
    {
        CancelTargeting();

        if (myTurn)
            usedThisTurn = false;
    }

    void OnRelicClicked()
    {
        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        bool canUse =
            TurnManager.Inst.myTurn &&
            !usedThisTurn &&
            TurnManager.Inst.CanPayMana(true, relic.manaCost);

        if (canUse &&
            relic.effectType == RelicEffectType.HealAlly2)
        {
            bool existHealTarget = false;

            foreach (var e in EntityManager.Inst.MyEntities)
            {
                if (e == null) continue;
                if (e.isDie) continue;
                if (e.isBossOrEmpty) continue;

                if (e.health < e.maxHealth)
                {
                    existHealTarget = true;
                    break;
                }
            }

            canUse = existHealTarget;
        }

        if (!canUse)
        {
            ShowTempGuide();
            return;
        }

        if (targeting)
            CancelTargeting();
        else
            StartTargeting();
    }

    void ShowTempGuide()
    {
        ShowTargetGuide(true);

        if (guideHideCo != null)
            StopCoroutine(guideHideCo);

        guideHideCo = StartCoroutine(HideGuideCo());
    }

    IEnumerator HideGuideCo()
    {
        yield return new WaitForSeconds(3f);

        if (!targeting)
            ShowTargetGuide(false);

        guideHideCo = null;
    }

    void StartTargeting()
    {
        targeting = true;

        ShowTargetMarks(true);

        ShowTargetGuide(true);
    }

    void CancelTargeting()
    {
        targeting = false;

        ShowTargetMarks(false);

        ShowTargetGuide(false);
    }

    void ShowTargetGuide(bool show)
    {
        if (targetGuideRoot == null)
            return;

        targetGuideRoot.SetActive(show);

        if (!show)
            return;

        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        if (targetGuideTMP != null)
            targetGuideTMP.text = relic.description;
    }

    void ShowTargetMarks(bool show)
    {
        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        bool enemyTarget =
            relic.effectType == RelicEffectType.DamageEnemy2;

        Entity[] entities =
            GameObject.FindObjectsOfType<Entity>();

        foreach (var e in entities)
        {
            if (e == null) continue;
            if (e.isDie) continue;
            if (e.isBossOrEmpty) continue;

            bool validTarget =
                enemyTarget
                ? !e.isMine
                : e.isMine;

            if (!validTarget)
            {
                e.SetRelicTargetMark(false);
                continue;
            }

            bool canTarget = true;

            if (!enemyTarget)
            {
                canTarget =
                    e.health < e.maxHealth;
            }

            e.SetRelicTargetMark(show && canTarget);
        }
    }


    public void OnEntitySelected(Entity entity)
    {
        if (!targeting)
            return;

        if (entity.isBossOrEmpty)
            return;

        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        bool enemyTarget =
            relic.effectType == RelicEffectType.DamageEnemy2;

        if (enemyTarget && entity.isMine)
            return;

        if (!enemyTarget && !entity.isMine)
            return;

        if (!enemyTarget &&
            entity.health >= entity.maxHealth)
            return;

        ApplyRelicEffect(entity);

        TurnManager.Inst.PayMana(true, relic.manaCost);

        ConsumeUse();

        ShowTargetMarks(false);

        ShowTargetGuide(false);

        if (BattleTutorialManager.Inst != null && BattleTutorialManager.Inst.IsActive)
        {
            BattleTutorialManager.Inst.OnRelicUsed(relic, entity);
        }
    }

    void ApplyRelicEffect(Entity entity)
    {
        RelicDataSO relic = GetCurrentRelic();

        if (relic == null)
            return;

        switch (relic.effectType)
        {
            case RelicEffectType.DamageEnemy2:

                entity.Damaged(2);

                EntityManager.Inst.ShowDamage(2, entity.transform);

                EntityManager.Inst.RemoveEntityIfDead(entity);

                break;

            case RelicEffectType.HealAlly2:

                EntityManager.Inst.PlayHealEffect(transform, entity, 2);

                break;
        }
    }

    public void ConsumeUse()
    {
        usedThisTurn = true;
        targeting = false;
    }

    RelicDataSO GetCurrentRelic()
    {
        if (BattleData.battleMode == BattleMode.Tutorial2 &&
            BattleTutorialManager.Inst != null &&
            BattleTutorialManager.Inst.Tutorial2Relic != null)
        {
            return BattleTutorialManager.Inst.Tutorial2Relic;
        }

        if (PlayerRelicManager.Inst == null)
            return null;

        return PlayerRelicManager.Inst.equippedRelic;
    }

}