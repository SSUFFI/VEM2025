using UnityEngine;

public enum SlateEffectType
{
    DelayEnemyMana = 0,
    IncreaseStartingMana = 1,

    SendEnemyDeckToGrave = 2,
    MultiplyRelicEffect = 3,
    GrantAllAlliesTaunt = 4,

    DrawStartingCards = 5,
    ReduceFirstHeroDamage = 6,
    BlockHeroDamage = 7,
    IncreaseFirstAllyHealth = 8,
    IncreaseFirstAllyAttack = 9,
    ReduceFirstSummonCost = 10,
    GrantGold = 11
}

[CreateAssetMenu(
    fileName = "SlateDataSO",
    menuName = "Scriptable Object/SlateDataSO")]
public class SlateDataSO : ScriptableObject
{
    [Header("혜택 정보")]
    public string rewardName;

    [TextArea(2, 5)]
    public string description;

    public Sprite rewardIcon;

    [Header("룬 표시")]
    [Tooltip("길손의 몫(Slate_0_01)만 해제합니다.")]
    public bool showRune = true;

    [Tooltip("비어 있으면 Reward Icon을 사용합니다.")]
    public Sprite runeIcon;

    [Header("필요 증표")]
    public TokenDataSO[] requiredTokens;

    [Header("적용 효과")]
    public SlateEffectType effectType;

    [Min(1)]
    public int effectValue = 1;

    [Header("금화 지급 효과에 사용할 아이템")]
    public ItemSO goldItem;

    public Sprite RuneIcon =>
        runeIcon != null ? runeIcon : rewardIcon;

    // 현재 석판 설명의 마지막 줄이 실제 효과 설명입니다.
    public string EffectDescription
    {
        get
        {
            if (string.IsNullOrWhiteSpace(description))
                return rewardName;

            string text = description.Trim();
            int lastLine = text.LastIndexOf('\n');

            return lastLine < 0
                ? text
                : text.Substring(lastLine + 1);
        }
    }
}