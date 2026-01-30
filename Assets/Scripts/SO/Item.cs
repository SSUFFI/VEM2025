using UnityEngine;
using System.Collections.Generic;


public enum EGraveTrigger
{
    DealMyAttackToDeckAttacker,
}

public enum EDeathTrigger
{
    Draw1OnCombatDeath,
}

[System.Serializable]
public class Item
{
    public string name;
    public int attack;
    public int health;
    public int manaCost;
    public Sprite sprite;

    public List<EGraveTrigger> graveTriggers = new List<EGraveTrigger>();

    public List<EDeathTrigger> deathTriggers = new List<EDeathTrigger>();
}
