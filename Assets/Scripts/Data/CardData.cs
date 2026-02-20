using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public string name;
    public int attack;
    public int health;
    public int manaCost;
    public Sprite sprite;

    public List<EGraveTrigger> graveTriggers = new List<EGraveTrigger>();
    public List<EDeathTrigger> deathTriggers = new List<EDeathTrigger>();
}