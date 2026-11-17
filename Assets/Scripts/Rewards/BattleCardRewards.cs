using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class BattleCardRewardEntry
{
    public CardDataSO card;
    public bool newlyUnlocked;
    public int temporaryCopies;
    public int gold;
}

public static class BattleCardRewards
{
    private static readonly List<CardDataSO> bonusCards =
        new List<CardDataSO>();

    private static readonly List<BattleCardRewardEntry> pending =
        new List<BattleCardRewardEntry>();

    private static string accountKey;
    private static bool pendingBoss;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        bonusCards.Clear();
        pending.Clear();

        accountKey = null;
        pendingBoss = false;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void EnsureAccount()
    {
        string key = AccountManager.GetAccountKey("CardPool");

        if (accountKey == key)
            return;

        bonusCards.Clear();
        pending.Clear();

        accountKey = key;
    }

    private static void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        EnsureAccount();

        if (scene.name == "Town")
        {
            // 보스 보상 팝업은 남기고 임시 덱만 초기화합니다.
            bonusCards.Clear();
        }

        if (scene.name == "Title")
        {
            bonusCards.Clear();
            pending.Clear();
        }
    }

    public static void ClearRunCards()
    {
        EnsureAccount();

        bonusCards.Clear();

        // 보스를 이기고 마을로 이동할 때도 호출되므로
        // 아직 표시하지 않은 보상 팝업은 지우지 않습니다.
    }

    public static void AppendRunCards(List<CardDataSO> deck)
    {
        EnsureAccount();

        if (deck != null)
            deck.AddRange(bonusCards);
    }

    public static bool IsUndead(CardDataSO card)
    {
        return card != null &&
               card.raceSprite != null &&
               string.Equals(
                   card.raceSprite.name,
                   "Race_Undead",
                   StringComparison.OrdinalIgnoreCase);
    }

    public static void GrantVictory(
        DeckSO enemy,
        bool boss)
    {
        EnsureAccount();

        if (BattleData.battleMode != BattleMode.Normal ||
            BattleData.isTutorialBattle)
        {
            return;
        }

        if (enemy == null ||
            enemy.deckItems == null ||
            CardPool.Inst == null)
        {
            Debug.LogError(
                "승리 카드 보상: 상대 덱 또는 CardPool이 없습니다.");

            return;
        }

        List<CardDataSO> candidates =
            new List<CardDataSO>();

        HashSet<string> ids =
            new HashSet<string>();

        // 남은 덱이 아니라 전투 시작 시 상대의 전체 카드풀을 사용합니다.
        foreach (CardDataSO card in enemy.deckItems)
        {
            if (card == null ||
                card.isSummonOnly ||
                string.IsNullOrEmpty(card.cardId))
            {
                continue;
            }

            if (boss)
            {
                // 보스전: 언데드만 후보로 사용합니다.
                if (!IsUndead(card))
                    continue;
            }
            else
            {
                // 일반전: 보스 전용 및 언데드 카드를 제외합니다.
                if (card.isBossOnly || IsUndead(card))
                    continue;
            }

            if (!CardPool.Inst.CanSaveRewardCard(card))
            {
                Debug.LogError(
                    "보상 카드가 CardPool의 Card Database에 없습니다: " +
                    card.name);

                continue;
            }

            // 상대 덱에 같은 카드가 여러 장 있어도 한 종류로 계산합니다.
            if (ids.Add(card.cardId))
                candidates.Add(card);
        }

        int rewardTypeCount = boss ? 2 : 1;

        if (candidates.Count < rewardTypeCount)
        {
            Debug.LogError(
                "승리 카드 보상 후보 부족: 필요 " +
                rewardTypeCount +
                ", 현재 " +
                candidates.Count);

            return;
        }

        // 보상을 변경하기 전에 중복 골드 지급 가능 여부를 확인합니다.
        bool canPayGold =
            InventoryManager.Inst != null &&
            enemy.goldItem != null;

        if (!canPayGold)
        {
            foreach (CardDataSO card in candidates)
            {
                if (CardPool.Inst.HasUnlockedCard(card))
                {
                    Debug.LogError(
                        "중복 보상 지급에 필요한 InventoryManager 또는 " +
                        "적 DeckSO의 Gold Item이 없습니다.");

                    return;
                }
            }
        }

        pending.Clear();
        pendingBoss = boss;

        for (int i = 0; i < rewardTypeCount; i++)
        {
            int index = UnityEngine.Random.Range(
                0,
                candidates.Count);

            CardDataSO card = candidates[index];

            // 보스전에서 같은 종류가 두 번 선택되지 않도록 제거합니다.
            candidates.RemoveAt(index);

            bool alreadyOwned =
                CardPool.Inst.HasUnlockedCard(card);

            int temporaryCopies = boss ? 0 : 4;
            int duplicateGold = alreadyOwned ? 30 : 0;

            if (!alreadyOwned)
                CardPool.Inst.AddCard(card);

            if (duplicateGold > 0)
            {
                InventoryManager.Inst.AddItem(
                    enemy.goldItem,
                    duplicateGold);
            }

            for (int j = 0; j < temporaryCopies; j++)
                bonusCards.Add(card);

            pending.Add(new BattleCardRewardEntry
            {
                card = card,
                newlyUnlocked = !alreadyOwned,
                temporaryCopies = temporaryCopies,
                gold = duplicateGold
            });
        }
    }

    public static bool TryGetNotice(
        bool town,
        out List<BattleCardRewardEntry> entries)
    {
        EnsureAccount();

        entries = null;

        if (pending.Count == 0 || pendingBoss != town)
            return false;

        entries =
            new List<BattleCardRewardEntry>(pending);

        return true;
    }

    public static void DismissNotice()
    {
        EnsureAccount();
        pending.Clear();
    }
}
