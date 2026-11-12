using UnityEngine;

[CreateAssetMenu(fileName = "TokenDataSO", menuName = "Scriptable Object/TokenDataSO")]
public class TokenDataSO : ScriptableObject
{
    public string tokenId;
    public string tokenName;
    public Sprite tokenIcon;
    [TextArea] public string description;

    public TokenSource source;
}

public enum TokenSource
{
    Explore,
    NPC
}