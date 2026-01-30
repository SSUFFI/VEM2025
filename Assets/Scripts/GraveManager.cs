using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveManager : MonoBehaviour
{
    public static GraveManager Inst;

    public List<Item> myGrave = new List<Item>();
    public List<Item> enemyGrave = new List<Item>();

    public static event Action<Item, bool, Entity> OnCardSentToGraveFromDeck;
    public static event Action<Item, bool, Entity, Entity> OnEntityDiedInCombat;

    void Awake()
    {
        Inst = this;
    }

    public void AddToGrave(Item item, bool isMine)
    {
        if (item == null) return;

        if (isMine)
            myGrave.Add(item); 
        else
            enemyGrave.Add(item);
    }

    public void AddToGraveFromDeck(Item item, bool isMineDeck, Entity deckAttacker)
    {
        AddToGrave(item, isMineDeck);
        OnCardSentToGraveFromDeck?.Invoke(item, isMineDeck, deckAttacker);
    }

    public void RaiseEntityDiedInCombat(Item item, bool isMine, Entity killer, Entity deadEntity)
    {
        OnEntityDiedInCombat?.Invoke(item, isMine, killer, deadEntity);
    }
}
