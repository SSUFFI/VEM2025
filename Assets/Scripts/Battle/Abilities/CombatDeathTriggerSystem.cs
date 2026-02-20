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

    void HandleEntityDiedInCombat(CardData deadData, bool deadIsMine, Entity killer, Entity deadEntity)
    {
        if (deadData == null) return;
        if (deadData.deathTriggers == null) return;

        if (!deadData.deathTriggers.Contains(EDeathTrigger.Draw1OnCombatDeath))
            return;

        TurnManager.OnAddCard?.Invoke(deadIsMine);
    }
}