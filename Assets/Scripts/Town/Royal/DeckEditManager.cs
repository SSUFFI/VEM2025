using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedDeckData
{
    public List<string> cardIds =
        new List<string>();
}

public class DeckEditManager : MonoBehaviour
{
    public static DeckEditManager Inst;

    public List<CardDataSO> currentDeck =
        new List<CardDataSO>();

    public List<CardDataSO> savedDeck =
        new List<CardDataSO>();

    public List<CardDataSO> fixedOrder =
        new List<CardDataSO>();

    [Header("Deck")]
    public int maxDeckSize = 40;

    [Header("Card Database")]
    [SerializeField] CardDatabaseSO cardDatabase;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);

            LoadSavedDeck();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // LOAD TO EDIT
    // =========================================================

    public void LoadDeck()
    {
        currentDeck =
            new List<CardDataSO>(savedDeck);

        fixedOrder.Clear();

        foreach (var card in currentDeck)
        {
            if (card == null)
                continue;

            if (!fixedOrder.Contains(card))
                fixedOrder.Add(card);
        }
    }

    // =========================================================
    // SAVE DECK
    // =========================================================

    public void SaveDeck()
    {
        savedDeck =
            new List<CardDataSO>(currentDeck);

        SaveSavedDeck();

        Debug.Log("덱 저장됨");

        if (currentDeck.Count == maxDeckSize)
        {
            if (TutorialManager.Inst != null)
                TutorialManager.Inst.OnDeckCompleted();
        }
    }

    void SaveSavedDeck()
    {
        string key =
            AccountManager.GetAccountKey(
                "SavedDeck");

        SavedDeckData saveData =
            new SavedDeckData();

        foreach (CardDataSO card in savedDeck)
        {
            if (card == null)
                continue;

            if (string.IsNullOrEmpty(card.cardId))
            {
                Debug.LogWarning(
                    $"{card.name}의 cardId가 없습니다.");

                continue;
            }

            // 중복 카드도 그대로 저장
            saveData.cardIds.Add(
                card.cardId);
        }

        string json =
            JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(
            key,
            json);

        PlayerPrefs.Save();

        Debug.Log(
            $"저장 덱 저장 완료 : {key}");
    }

    // =========================================================
    // LOAD SAVED DECK
    // =========================================================

    public void LoadSavedDeck()
    {
        savedDeck.Clear();
        currentDeck.Clear();
        fixedOrder.Clear();

        if (cardDatabase == null)
        {
            Debug.LogWarning(
                "DeckEditManager에 CardDatabaseSO가 연결되지 않았습니다.");

            return;
        }

        string key =
            AccountManager.GetAccountKey(
                "SavedDeck");

        if (!PlayerPrefs.HasKey(key))
        {
            Debug.Log(
                $"저장된 덱이 없습니다 : {key}");

            return;
        }

        string json =
            PlayerPrefs.GetString(
                key,
                "");

        if (string.IsNullOrEmpty(json))
            return;

        SavedDeckData saveData =
            JsonUtility.FromJson<SavedDeckData>(
                json);

        if (saveData == null ||
            saveData.cardIds == null)
            return;

        foreach (string cardId in saveData.cardIds)
        {
            CardDataSO card =
                cardDatabase.FindById(cardId);

            if (card == null)
            {
                Debug.LogWarning(
                    $"저장 덱의 카드를 찾을 수 없습니다. ID: {cardId}");

                continue;
            }

            // 같은 카드가 4장이라면
            // 4번 그대로 추가됨
            savedDeck.Add(card);
        }

        Debug.Log(
            $"저장 덱 불러오기 완료 : {savedDeck.Count}장");
    }

    // =========================================================
    // EDIT
    // =========================================================

    public void ClearDeck()
    {
        currentDeck.Clear();
        fixedOrder.Clear();
    }

    public bool AddCard(CardDataSO data)
    {
        if (data == null)
            return false;

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
        if (data == null)
            return;

        currentDeck.Remove(data);

        if (!currentDeck.Contains(data))
            fixedOrder.Remove(data);
    }

    // =========================================================
    // AUTO BUILD
    // =========================================================

    public void AutoBuildRandomDeck()
    {
        ClearDeck();

        if (CardPool.Inst == null)
            return;

        List<CardDataSO> candidates =
            new List<CardDataSO>(
                CardPool.Inst.ownedCards);

        while (currentDeck.Count < maxDeckSize)
        {
            List<CardDataSO> available =
                new List<CardDataSO>();

            foreach (var card in candidates)
            {
                if (card == null)
                    continue;

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

            CardDataSO randomCard =
                available[
                    Random.Range(
                        0,
                        available.Count)];

            AddCard(randomCard);
        }

        SaveDeck();

        if (DeckListUI.Inst != null)
            DeckListUI.Inst.Refresh();
    }
}