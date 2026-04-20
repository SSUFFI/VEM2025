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

    public List<EGraveTrigger> graveTriggers = new List<EGraveTrigger>();
    public List<EDeathTrigger> deathTriggers = new List<EDeathTrigger>();
}