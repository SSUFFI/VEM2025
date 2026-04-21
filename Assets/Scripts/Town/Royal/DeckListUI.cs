using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckListUI : MonoBehaviour
{
    public static DeckListUI Inst;

    public Transform content;
    public GameObject deckCardPrefab;
    public TMP_Text deckCountTMP;

    bool isHolding;
    float holdTime;
    float nextTime;
    CardDataSO currentData;

    bool isAddMode;

    void Awake()
    {
        Inst = this;
    }

    void Update()
    {
        if (!isHolding) return;

        if (!Input.GetMouseButton(1))
        {
            StopHold();
            return;
        }

        if (DeckEditManager.Inst == null || currentData == null)
        {
            StopHold();
            return;
        }

        holdTime += Time.deltaTime;

        float interval = Mathf.Lerp(0.35f, 0.12f, holdTime * 1.2f);
        interval = Mathf.Max(0.08f, interval);

        if (Time.time >= nextTime)
        {
            if (isAddMode)
                AddOnce(currentData);
            else
                RemoveOnce(currentData);

            nextTime = Time.time + interval;
        }
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

    public void StartRemove(CardDataSO data)
    {
        RemoveOnce(data);
    }

    public void StartHoldRemove(CardDataSO data)
    {
        currentData = data;
        isHolding = true;
        holdTime = 0f;
        nextTime = Time.time + 0.35f;
        isAddMode = false;
    }

    void RemoveOnce(CardDataSO data)
    {
        if (DeckEditManager.Inst == null) return;

        DeckEditManager.Inst.RemoveCard(data);
        Refresh();
    }

    public void StartAdd(CardDataSO data)
    {
        AddOnce(data);
    }

    public void StartHoldAdd(CardDataSO data)
    {
        currentData = data;
        isHolding = true;
        holdTime = 0f;
        nextTime = Time.time + 0.35f;
        isAddMode = true;
    }

    void AddOnce(CardDataSO data)
    {
        if (DeckEditManager.Inst == null) return;

        if (DeckEditManager.Inst.AddCard(data))
            Refresh();
    }

    public void StopHold()
    {
        isHolding = false;
    }
}