
using UnityEngine;

public enum SlateEffectType
{
    DelayEnemyMana,
    IncreaseStartingMana,
    SendEnemyDeckToGrave,
    MultiplyRelicEffect,
    GrantAllAlliesTaunt
}

[CreateAssetMenu(fileName = "SlateDataSO", menuName = "Scriptable Object/SlateDataSO")]
public class SlateDataSO : ScriptableObject
{
    [Header("혜택 정보")]
    public string rewardName;

    [TextArea(2, 5)]
    public string description;

    public Sprite rewardIcon;

    [Header("필요 증표 (0~3개)")]
    public TokenDataSO[] requiredTokens;

    [Header("보스전 적용 효과")]
    public SlateEffectType effectType;

    [Min(1)]
    public int effectValue = 1;
}
