using System.Collections.Generic;
using UnityEngine;

public class DeckEditManager : MonoBehaviour
{
    public static DeckEditManager Inst;

    public List<CardDataSO> currentDeck = new List<CardDataSO>(); 
    public List<CardDataSO> savedDeck = new List<CardDataSO>(); 

    public List<CardDataSO> fixedOrder = new List<CardDataSO>();

    public int maxDeckSize = 40;

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

    public void LoadDeck()
    {
        currentDeck = new List<CardDataSO>(savedDeck);

        fixedOrder.Clear();

        foreach (var card in currentDeck)
        {
            if (!fixedOrder.Contains(card))
                fixedOrder.Add(card);
        }
    }

    public void SaveDeck()
    {
        savedDeck = new List<CardDataSO>(currentDeck);
        Debug.Log("µ¦ ÀúÀåµÊ");

        if (currentDeck.Count == maxDeckSize)
        {
            if (TutorialManager.Inst != null)
                TutorialManager.Inst.OnDeckCompleted();
        }
    }

    public void ClearDeck()
    {
        currentDeck.Clear();
        fixedOrder.Clear();
    }

    public bool AddCard(CardDataSO data)
    {
        if (currentDeck.Count >= maxDeckSize)
            return false;

        int count = 0;

        foreach (var card in currentDeck)
        {
            if (card == data)
                count++;
        }

        if (count >= 4)
            return false;

        currentDeck.Add(data);

        if (!fixedOrder.Contains(data))
            fixedOrder.Add(data);

        return true;
    }

    public void RemoveCard(CardDataSO data)
    {
        currentDeck.Remove(data);

        if (!currentDeck.Contains(data))
            fixedOrder.Remove(data);
    }

    public void AutoBuildRandomDeck()
    {
        ClearDeck();

        if (CardPool.Inst == null)
            return;

        List<CardDataSO> candidates = new List<CardDataSO>(CardPool.Inst.ownedCards);

        while (currentDeck.Count < maxDeckSize)
        {
            List<CardDataSO> available = new List<CardDataSO>();

            foreach (var card in candidates)
            {
                int count = 0;

                foreach (var deckCard in currentDeck)
                {
                    if (deckCard == card)
                        count++;
                }

                if (count < 4)
                    available.Add(card);
            }

            if (available.Count == 0)
                break;

            CardDataSO randomCard = available[Random.Range(0, available.Count)];

            AddCard(randomCard);
        }

        SaveDeck();

        if (DeckListUI.Inst != null)
            DeckListUI.Inst.Refresh();
    }

}