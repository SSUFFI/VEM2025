using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NodeData")]
public class NodeDataSO : ScriptableObject
{
    public NodeType nodeType;

    [Header("Battle")]
    public BattleTier battleTier;
    public DeckSO enemyDeck;

    [Header("Event")]
    public EventType eventType;

    [Header("Icon")]
    public Sprite iconNormal;
    public Sprite iconGray;
}

public enum NodeType
{
    Start,
    Battle,
    Event,
    Boss
}

public enum BattleTier
{
    None,
    Tier1,
    Tier2,
    Tier3
}

public enum EventType
{
    None,
    NPC,
    Heal,
    Explore,
    Tablet
}