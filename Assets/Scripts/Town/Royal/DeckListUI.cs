using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckListUI : MonoBehaviour
{
    public static DeckListUI Inst;

    public Transform content;
    public GameObject deckCardPrefab;

    public TMP_Text deckCountTMP;

    void Awake()
    {
        Inst = this;
    }

    public void Refresh()
    {
        if (DeckEditManager.Inst == null) return;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        var deck = DeckEditManager.Inst.currentDeck;
        var order = DeckEditManager.Inst.fixedOrder;

        Dictionary<CardDataSO, int> countDict = new Dictionary<CardDataSO, int>();

        foreach (var card in deck)
        {
            if (!countDict.ContainsKey(card))
                countDict[card] = 0;

            countDict[card]++;
        }

        foreach (var card in order)
        {
            if (!countDict.ContainsKey(card)) continue;

            var go = Instantiate(deckCardPrefab, content);

            var ui = go.GetComponent<DeckCardUI>();
            ui.Init(card, countDict[card]);
        }

        deckCountTMP.text = $"{deck.Count} / {DeckEditManager.Inst.maxDeckSize}";
    }
}