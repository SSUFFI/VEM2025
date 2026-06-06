using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DeckRewardItem
{
    public ItemSO item;
    public int count = 1;
}

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Object/DeckSO")]
public class DeckSO : ScriptableObject
{
    public CardDataSO[] deckItems;

    [Header("Enemy Hero Portrait")]
    public Sprite heroPortrait;

    [Header("Battle Reward")]
    public ItemSO goldItem;
    public int goldAmount = 0;

    [Tooltip("최대 4개까지만 사용")]
    public List<DeckRewardItem> rewardItems = new List<DeckRewardItem>();
}