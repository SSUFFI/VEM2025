using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public static CardPool Inst;

    public List<CardDataSO> ownedCards = new List<CardDataSO>();

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCard(CardDataSO card)
    {
        if (!ownedCards.Contains(card))
            ownedCards.Add(card);
    }
}