using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveManager : MonoBehaviour
{
    public static GraveManager Inst;

    public List<CardDataSO> myGrave = new List<CardDataSO>();
    public List<CardDataSO> enemyGrave = new List<CardDataSO>();

    public static event Action<CardDataSO, bool, Entity> OnCardSentToGraveFromDeck;
    public static event Action<CardDataSO, bool, Entity, Entity> OnEntityDiedInCombat;

    void Awake()
    {
        Inst = this;
    }

    public void AddToGrave(CardDataSO data, bool isMine)
    {
        if (data == null) return;

        if (isMine)
            myGrave.Add(data);
        else
            enemyGrave.Add(data);
    }

    public void AddToGraveFromDeck(CardDataSO data, bool isMineDeck, Entity deckAttacker)
    {
        AddToGrave(data, isMineDeck);
        OnCardSentToGraveFromDeck?.Invoke(data, isMineDeck, deckAttacker);
    }

    public void RaiseEntityDiedInCombat(CardDataSO data, bool isMine, Entity killer, Entity deadEntity)
    {
        OnEntityDiedInCombat?.Invoke(data, isMine, killer, deadEntity);
    }
}