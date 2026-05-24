using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerSystem : MonoBehaviour
{
    public static TriggerSystem Inst;

    void Awake()
    {
        Inst = this;
    }

    void OnEnable()
    {
        GraveManager.OnCardSentToGraveFromDeck += OnDeckToGrave;
        GraveManager.OnEntityDiedInCombat += OnFieldDeath;
    }

    void OnDisable()
    {
        GraveManager.OnCardSentToGraveFromDeck -= OnDeckToGrave;
        GraveManager.OnEntityDiedInCombat -= OnFieldDeath;
    }

    public void OnEnterField(CardDataSO data, bool isMine, Entity self)
    {
        if (data == null) return;
        if (data.triggers == null) return;

        foreach (var trigger in data.triggers)
        {
            if (trigger.triggerType != TriggerType.OnEnterField)
                continue;

            RunEffect(trigger, data, isMine, null, self, null);
        }
    }

    void OnDeckToGrave(CardDataSO data, bool isMineDeck, Entity deckAttacker)
    {
        if (data == null) return;
        if (data.triggers == null) return;

        foreach (var trigger in data.triggers)
        {
            if (trigger.triggerType != TriggerType.OnDeckToGrave)
                continue;

            RunEffect(trigger, data, isMineDeck, deckAttacker, null, null);
        }
    }

    void OnFieldDeath(CardDataSO deadData, bool deadIsMine, Entity killer, Entity deadEntity)
    {
        if (deadData == null) return;
        if (deadData.triggers == null) return;

        foreach (var trigger in deadData.triggers)
        {
            if (trigger.triggerType != TriggerType.OnFieldDeath)
                continue;

            RunEffect(trigger, deadData, deadIsMine, killer, deadEntity, null);
        }
    }

    void RunEffect(
        CardTriggerData trigger,
        CardDataSO data,
        bool isMine,
        Entity target,
        Entity self,
        Entity extra)
    {
        switch (trigger.effectType)
        {
            case EffectType.None:
                break;

            case EffectType.DealOwnAttackToAttacker:
                DealOwnAttackToAttacker(data, target);
                break;

            case EffectType.Draw1:
                TurnManager.OnAddCard?.Invoke(isMine);
                break;

            case EffectType.Summon:
                Summon(trigger, isMine, self);
                break;

            case EffectType.DamageRandomEnemy:
                DamageRandomEnemy(trigger.value, isMine);
                break;

            case EffectType.DamageAllEnemies:
                DamageAllEnemies(trigger.value, isMine);
                break;

            case EffectType.HealRandomAlly:
                HealRandomAlly(trigger.value, isMine, self);
                break;

            case EffectType.HealAllAllies:
                HealAllAllies(trigger.value, isMine, self);
                break;
        }
    }

    void Summon(CardTriggerData trigger, bool isMine, Entity owner)
    {
        StartCoroutine(SummonCo(trigger, isMine, owner));
    }

    IEnumerator SummonCo(CardTriggerData trigger, bool isMine, Entity owner)
    {
        if (trigger.summonCards == null || trigger.summonCards.Count == 0)
            yield break;

        if (EntityManager.Inst == null)
            yield break;

        bool isDeathSummon = owner != null && owner.isDie;

        Entity safeOwner = owner;

        if (owner != null && owner.isDie)
        {
            safeOwner = null;
        }

        EntityManager.Inst.SetSummonOwner(owner, isDeathSummon);

        if (owner != null && !owner.isDie)
            yield return new WaitForSeconds(0.8f);

        foreach (var summonData in trigger.summonCards)
        {
            if (summonData == null)
                continue;

            Vector3 spawnPos = owner != null
                ? owner.transform.position
                : (isMine
                    ? EntityManager.Inst.MyDeckSpawnPos
                    : EntityManager.Inst.OtherDeckSpawnPos);

            bool success = EntityManager.Inst.SpawnEntity(
                isMine,
                summonData,
                spawnPos
            );

            if (!success)
                break;

            yield return new WaitForSeconds(0.12f);
        }

        EntityManager.Inst.ClearSummonOwner();

        EntityManager.Inst.EntityAlignment(isMine);
    }

    void DealOwnAttackToAttacker(CardDataSO data, Entity attacker)
    {
        if (data == null || attacker == null) return;
        if (attacker.isDie) return;

        int dmg = Mathf.Max(0, data.attack);
        if (dmg <= 0) return;

        attacker.Damaged(dmg);

        if (EntityManager.Inst != null)
            EntityManager.Inst.ShowDamage(dmg, attacker.transform);

        if (EntityManager.Inst != null)
            EntityManager.Inst.RemoveEntityIfDead(attacker);
    }

    void DamageRandomEnemy(int damage, bool isMine)
    {
        var list = isMine
            ? EntityManager.Inst.OtherEntities
            : EntityManager.Inst.MyEntities;

        List<Entity> targets = new List<Entity>();

        foreach (var e in list)
        {
            if (e == null) continue;
            if (e.isDie) continue;
            if (e.isBossOrEmpty) continue;

            targets.Add(e);
        }

        if (targets.Count == 0) return;

        var target = targets[Random.Range(0, targets.Count)];

        target.Damaged(damage);

        EntityManager.Inst.ShowDamage(damage, target.transform);
        EntityManager.Inst.RemoveEntityIfDead(target);
    }

    void DamageAllEnemies(int damage, bool isMine)
    {
        var list = isMine
            ? EntityManager.Inst.OtherEntities
            : EntityManager.Inst.MyEntities;

        List<Entity> targets = new List<Entity>(list);

        foreach (var e in targets)
        {
            if (e == null) continue;
            if (e.isDie) continue;
            if (e.isBossOrEmpty) continue;

            e.Damaged(damage);

            EntityManager.Inst.ShowDamage(damage, e.transform);
            EntityManager.Inst.RemoveEntityIfDead(e);
        }
    }

    void HealRandomAlly(int healValue, bool isMine, Entity self)
    {
        if (EntityManager.Inst == null) return;
        if (healValue <= 0) return;

        var list = isMine
            ? EntityManager.Inst.MyEntities
            : EntityManager.Inst.OtherEntities;

        List<Entity> targets = new List<Entity>();

        foreach (var e in list)
        {
            if (e == null) continue;
            if (!e.CanBeHealed()) continue;

            targets.Add(e);
        }

        if (targets.Count == 0) return;

        Entity target = targets[Random.Range(0, targets.Count)];

        Transform from;

        if (self != null)
        {
            from = self.transform;
        }
        else
        {
            Vector3 deckPos = isMine
                ? EntityManager.Inst.MyDeckSpawnPos
                : EntityManager.Inst.OtherDeckSpawnPos;

            GameObject temp = new GameObject("HealFrom");

            temp.transform.position = deckPos;

            from = temp.transform;

            Object.Destroy(temp, 1f);
        }

        EntityManager.Inst.PlayHealEffect(from, target, healValue);
    }

    void HealAllAllies(int healValue, bool isMine, Entity self)
    {
        if (EntityManager.Inst == null) return;
        if (healValue <= 0) return;

        var list = isMine
            ? EntityManager.Inst.MyEntities
            : EntityManager.Inst.OtherEntities;

        Transform from = null;

        if (self != null)
        {
            from = self.transform;
        }
        else
        {
            Vector3 deckPos = isMine
                ? EntityManager.Inst.MyDeckSpawnPos
                : EntityManager.Inst.OtherDeckSpawnPos;

            GameObject temp = new GameObject("HealFrom");

            temp.transform.position = deckPos;

            from = temp.transform;

            Object.Destroy(temp, 1f);
        }

        foreach (var e in list)
        {
            if (e == null) continue;
            if (!e.CanBeHealed()) continue;

            EntityManager.Inst.PlayHealEffect(
                from != null ? from : e.transform,
                e,
                healValue
            );
        }
    }
}