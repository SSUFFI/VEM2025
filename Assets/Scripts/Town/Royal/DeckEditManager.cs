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
    }

    public bool AddCard(CardDataSO data)
    {
        if (currentDeck.Count >= maxDeckSize)
        {
            Debug.Log("µ¦ °¡µæ Âü");
            return false;
        }

        currentDeck.Add(data);

        if (!fixedOrder.Contains(data))
            fixedOrder.Add(data);

        CheckDeckComplete();

        return true;
    }

    public void RemoveCard(CardDataSO data)
    {
        currentDeck.Remove(data);

        if (!currentDeck.Contains(data))
            fixedOrder.Remove(data);
    }

    public void CheckDeckComplete()
    {
        if (currentDeck.Count >= maxDeckSize)
        {
            if (TutorialManager.Inst != null)
                TutorialManager.Inst.OnDeckCompleted();
        }
    }
}