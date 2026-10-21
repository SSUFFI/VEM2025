
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "CardDataSO",
    menuName = "Scriptable Object/CardDataSO")]
public class CardDataSO : ScriptableObject
{
    [Header("ID")]
    public string cardId;

    public string cardName;
    public int attack;
    public int health;
    public int manaCost;

    public Sprite sprite;
    public Sprite fieldSprite;

    [Header("종족")]
    public Sprite raceSprite;

    [Header("Descriptions")]
    [TextArea]
    public string descriptionTop;

    [TextArea]
    public string descriptionMiddle;

    [TextArea]
    public string descriptionBottom;

    [TextArea]
    public string descriptionFloor;

    public List<string> keywords =
        new List<string>();

    [Header("도발")]
    public bool taunt;

    public List<CardTriggerData> triggers =
        new List<CardTriggerData>();
}

[System.Serializable]
public class CardTriggerData
{
    public TriggerType triggerType;
    public EffectType effectType;
    public int value;

    public List<CardDataSO> summonCards =
        new List<CardDataSO>();
}

public enum TriggerType
{
    OnEnterField,
    OnDeckToGrave,
    OnFieldDeath
}

public enum EffectType
{
    None,
    DealOwnAttackToAttacker,
    Draw1,
    Summon,
    DamageRandomEnemy,
    DamageAllEnemies,
    HealRandomAlly,
    HealAllAllies
}
