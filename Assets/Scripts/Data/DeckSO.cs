using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Object/DeckSO")]
public class DeckSO : ScriptableObject
{
    public CardData[] deckItems;
}