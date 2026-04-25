using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Object/DeckSO")]
public class DeckSO : ScriptableObject
{
    public CardDataSO[] deckItems;

    [Header("Enemy Hero Portrait")]
    public Sprite heroPortrait;
}