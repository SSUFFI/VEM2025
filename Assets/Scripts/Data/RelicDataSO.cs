using UnityEngine;

public enum RelicEffectType
{
    DamageEnemy2,
    HealAlly2
}

[CreateAssetMenu(fileName = "RelicDataSO", menuName = "Scriptable Object/RelicDataSO")]
public class RelicDataSO : ScriptableObject
{
    [Header("Info")]
    public string relicName;

    [TextArea]
    public string description;

    [Header("Visual")]
    public Sprite icon;

    [Header("Battle UI")]
    public Sprite battleButtonSprite;

    [Header("Effect")]
    public RelicEffectType effectType;

    [Header("Gameplay")]
    public int manaCost;

    public bool isLocked;
}