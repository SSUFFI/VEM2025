using UnityEngine;

public class TriggerSystem : MonoBehaviour
{
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

    void OnDeckToGrave(CardDataSO data, bool isMineDeck, Entity deckAttacker)
    {
        if (data == null) return;

        TrySummonJudgement(data, isMineDeck);

        if (data.triggers == null) return;

        foreach (var trigger in data.triggers)
        {
            if (trigger.triggerType != TriggerType.OnDeckToGrave)
                continue;

            RunEffect(trigger.effectType, data, isMineDeck, deckAttacker, null, null);
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

            RunEffect(trigger.effectType, deadData, deadIsMine, killer, deadEntity, null);
        }
    }

    void RunEffect(
        EffectType effect,
        CardDataSO data,
        bool isMine,
        Entity target,
        Entity self,
        Entity extra)
    {
        switch (effect)
        {
            case EffectType.None:
                break;

            case EffectType.DealOwnAttackToAttacker:
                DealOwnAttackToAttacker(data, target);
                break;

            case EffectType.Draw1:
                TurnManager.OnAddCard?.Invoke(isMine);
                break;
        }
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

    void TrySummonJudgement(CardDataSO data, bool isMine)
    {
        if (data.summonOnDeckToGraveCards == null) return;
        if (data.summonOnDeckToGraveCards.Count == 0) return;
        if (EntityManager.Inst == null) return;

        foreach (var summonData in data.summonOnDeckToGraveCards)
        {
            if (summonData == null)
                continue;

            if (isMine)
                EntityManager.Inst.InsertMyEmptyEntity(999f);

            Vector3 spawnPos =
                isMine
                ? EntityManager.Inst.MyDeckSpawnPos
                : EntityManager.Inst.OtherDeckSpawnPos;

            bool success = EntityManager.Inst.SpawnEntity(
                isMine,
                summonData,
                spawnPos
            );

            if (!success)
                break;
        }
    }
}