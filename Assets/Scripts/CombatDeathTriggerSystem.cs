using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDeathTriggerSystem : MonoBehaviour
{
    void OnEnable()
    {
        GraveManager.OnEntityDiedInCombat += HandleEntityDiedInCombat;
    }

    void OnDisable()
    {
        GraveManager.OnEntityDiedInCombat -= HandleEntityDiedInCombat;
    }

    void HandleEntityDiedInCombat(Item deadItem, bool deadIsMine, Entity killer, Entity deadEntity)
    {
        if (deadItem == null) return;
        if (deadItem.deathTriggers == null) return;

        if (!deadItem.deathTriggers.Contains(EDeathTrigger.Draw1OnCombatDeath))
            return;

        TurnManager.OnAddCard?.Invoke(deadIsMine);
    }
}