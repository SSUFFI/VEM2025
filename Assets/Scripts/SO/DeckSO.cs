using UnityEngine;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Object/DeckSO")]
public class DeckSO : ScriptableObject
{
    public Item[] deckItems;
}
