using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveTriggerSystem : MonoBehaviour
{
    void OnEnable()
    {
        GraveManager.OnCardSentToGraveFromDeck += HandleSentToGraveFromDeck;
    }

    void OnDisable()
    {
        GraveManager.OnCardSentToGraveFromDeck -= HandleSentToGraveFromDeck;
    }

    void HandleSentToGraveFromDeck(CardData data, bool isMineDeck, Entity deckAttacker)
    {
        if (data == null || deckAttacker == null) return;
        if (deckAttacker.isDie) return;

        if (data.graveTriggers == null) return;
        if (!data.graveTriggers.Contains(EGraveTrigger.DealMyAttackToDeckAttacker)) return;

        int dmg = Mathf.Max(0, data.attack);
        if (dmg <= 0) return;

        deckAttacker.Damaged(dmg);

        if (EntityManager.Inst != null)
            EntityManager.Inst.ShowDamage(dmg, deckAttacker.transform);

        if (EntityManager.Inst != null)
            EntityManager.Inst.RemoveEntityIfDead(deckAttacker);
    }
}