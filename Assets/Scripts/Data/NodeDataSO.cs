using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NodeData")]
public class NodeDataSO : ScriptableObject
{
    public NodeType nodeType;

    [Header("Battle")]
    public DeckSO enemyDeck;

    [Header("Extra")]
    public bool isElite;
    public bool isBoss;
}

public enum NodeType
{
    Battle,
    Shop,
    Event
}