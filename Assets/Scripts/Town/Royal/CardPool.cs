using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPoolSaveData
{
    public List<string> cardIds =
        new List<string>();
}

public class CardPool : MonoBehaviour
{
    public static CardPool Inst;

    [Header("Owned Cards")]
    public List<CardDataSO> ownedCards =
        new List<CardDataSO>();

    [Header("Card Database")]
    [SerializeField] CardDatabaseSO cardDatabase;

    List<CardDataSO> startingCards =
        new List<CardDataSO>();

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);

            startingCards =
                new List<CardDataSO>(ownedCards);

            LoadCardPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddCard(CardDataSO card)
    {
        if (card == null)
            return;

        if (ownedCards.Contains(card))
            return;

        ownedCards.Add(card);

        SaveCardPool();
    }


    public void SaveCardPool()
    {
        string key =
            AccountManager.GetAccountKey(
                "CardPool");

        CardPoolSaveData saveData =
            new CardPoolSaveData();

        foreach (CardDataSO card in ownedCards)
        {
            if (card == null)
                continue;

            if (string.IsNullOrEmpty(card.cardId))
            {
                Debug.LogWarning(
                    $"{card.name}의 cardId가 없습니다.");

                continue;
            }

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
            $"카드 풀 저장 완료 : {key}");
    }


    public void LoadCardPool()
    {
        if (cardDatabase == null)
        {
            Debug.LogWarning(
                "CardPool에 CardDatabaseSO가 연결되지 않았습니다.");

            return;
        }

        string key =
            AccountManager.GetAccountKey(
                "CardPool");



        if (!PlayerPrefs.HasKey(key))
        {
            ownedCards =
                new List<CardDataSO>(
                    startingCards);


            SaveCardPool();

            Debug.Log(
                $"새 계정 초기 카드 지급 : {ownedCards.Count}장");

            return;
        }


        ownedCards.Clear();

        string json =
            PlayerPrefs.GetString(
                key,
                "");

        if (string.IsNullOrEmpty(json))
            return;

        CardPoolSaveData saveData =
            JsonUtility.FromJson<CardPoolSaveData>(
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
                    $"CardDataSO를 찾을 수 없습니다. ID: {cardId}");

                continue;
            }

            if (!ownedCards.Contains(card))
                ownedCards.Add(card);
        }

        Debug.Log(
            $"카드 풀 불러오기 완료 : {ownedCards.Count}장");
    }



    public void ClearCardPool()
    {
        ownedCards.Clear();

        SaveCardPool();
    }
}