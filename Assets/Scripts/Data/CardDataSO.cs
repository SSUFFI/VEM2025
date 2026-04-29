using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDataSO", menuName = "Scriptable Object/CardDataSO")]
public class CardDataSO : ScriptableObject
{
    public string cardName;
    public int attack;
    public int health;
    public int manaCost;
    public Sprite sprite;
    public Sprite fieldSprite;

    [TextArea]
    public string description;

    public List<string> keywords = new List<string>();

    [Header("Keyword")]
    public bool taunt;

    [Header("Summon - Play")]
    public List<CardDataSO> summonOnPlayCards = new List<CardDataSO>();

    [Header("Summon - Judgement")]
    public List<CardDataSO> summonOnDeckToGraveCards = new List<CardDataSO>();

    public List<CardTriggerData> triggers = new List<CardTriggerData>();
}

[System.Serializable]
public class CardTriggerData
{
    public TriggerType triggerType;
    public EffectType effectType;
}

public enum TriggerType
{
    OnDeckToGrave,
    OnFieldDeath
}

public enum EffectType
{
    None,

    DealOwnAttackToAttacker,
    Draw1
}