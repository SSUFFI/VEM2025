using System.Collections.Generic;
using UnityEngine;

public class EventTokenManager : MonoBehaviour
{
    public static EventTokenManager Instance;

    [SerializeField] private List<TokenDataSO> allTokens = new();
    private readonly List<TokenDataSO> ownedTokens = new();

    public IReadOnlyList<TokenDataSO> OwnedTokens => ownedTokens;

    public event System.Action OnTokensChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<TokenDataSO> GetTokensBySource(TokenSource source)
    {
        return allTokens.FindAll(token =>
            token != null && token.source == source);
    }

    public bool HasToken(TokenDataSO token)
    {
        return token != null && ownedTokens.Contains(token);
    }

    public bool AddToken(TokenDataSO token)
    {
        if (token == null || HasToken(token))
            return false;

        ownedTokens.Add(token);
        OnTokensChanged?.Invoke();

        return true;
    }

    public void ClearTokens()
    {
        ownedTokens.Clear();
        OnTokensChanged?.Invoke();
    }
}
