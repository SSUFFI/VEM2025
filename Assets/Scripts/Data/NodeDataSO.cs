using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NodeDataSO")]
public class NodeDataSO : ScriptableObject
{
    public NodeType nodeType;

    [Header("Battle")]
    public BattleTier battleTier;
    public DeckSO enemyDeck;

    [Header("Event")]
    public EventType eventType;

    //JS юс╫ц
    [Header("Event Preview")]
    public string eventTitle;

    public Sprite eventPreviewImage;

    [TextArea(3, 8)]
    public string eventDescription;
    //===

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