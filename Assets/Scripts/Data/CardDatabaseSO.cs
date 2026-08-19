using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Scriptable Object/Card Database",
    fileName = "CardDatabaseSO")]
public class CardDatabaseSO : ScriptableObject
{
    public List<CardDataSO> cards =
        new List<CardDataSO>();

    public CardDataSO FindById(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
            return null;

        return cards.Find(
            x => x != null &&
                 x.cardId == cardId);
    }
}