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

    void HandleSentToGraveFromDeck(Item item, bool isMineDeck, Entity deckAttacker)
    {
        if (item == null || deckAttacker == null) return;
        if (deckAttacker.isDie) return;

        if (item.graveTriggers == null) return;
        if (!item.graveTriggers.Contains(EGraveTrigger.DealMyAttackToDeckAttacker)) return;

        int dmg = Mathf.Max(0, item.attack);
        if (dmg <= 0) return;

        deckAttacker.Damaged(dmg);

        if (EntityManager.Inst != null)
            EntityManager.Inst.ShowDamage(dmg, deckAttacker.transform);

        if (EntityManager.Inst != null)
            EntityManager.Inst.RemoveEntityIfDead(deckAttacker);
    }
}