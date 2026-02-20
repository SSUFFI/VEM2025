using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveManager : MonoBehaviour
{
    public static GraveManager Inst;

    public List<CardData> myGrave = new List<CardData>();
    public List<CardData> enemyGrave = new List<CardData>();

    public static event Action<CardData, bool, Entity> OnCardSentToGraveFromDeck;
    public static event Action<CardData, bool, Entity, Entity> OnEntityDiedInCombat;

    void Awake()
    {
        Inst = this;
    }

    public void AddToGrave(CardData data, bool isMine)
    {
        if (data == null) return;

        if (isMine)
            myGrave.Add(data);
        else
            enemyGrave.Add(data);
    }

    public void AddToGraveFromDeck(CardData data, bool isMineDeck, Entity deckAttacker)
    {
        AddToGrave(data, isMineDeck);
        OnCardSentToGraveFromDeck?.Invoke(data, isMineDeck, deckAttacker);
    }

    public void RaiseEntityDiedInCombat(CardData data, bool isMine, Entity killer, Entity deadEntity)
    {
        OnEntityDiedInCombat?.Invoke(data, isMine, killer, deadEntity);
    }
}