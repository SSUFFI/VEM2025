
using UnityEngine;
using UnityEngine.UI;

public class EventTokenUI : MonoBehaviour
{
    [Header("Å½»ö ÁõÇ¥ 2Ä­")]
    [SerializeField] private Image exploreToken1;
    [SerializeField] private Image exploreToken2;

    [Header("Á¶¿ì ÁõÇ¥ 1Ä­")]
    [SerializeField] private Image npcToken;

    private void OnEnable()
    {
        if (EventTokenManager.Instance == null)
            return;

        EventTokenManager.Instance.OnTokensChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (EventTokenManager.Instance != null)
            EventTokenManager.Instance.OnTokensChanged -= Refresh;
    }

    public void Refresh()
    {
        ClearSlot(exploreToken1);
        ClearSlot(exploreToken2);
        ClearSlot(npcToken);

        if (EventTokenManager.Instance == null)
            return;

        int exploreCount = 0;

        foreach (TokenDataSO token in
                 EventTokenManager.Instance.OwnedTokens)
        {
            if (token == null)
                continue;

            if (token.source == TokenSource.Explore)
            {
                if (exploreCount == 0)
                    SetSlot(exploreToken1, token);

                else if (exploreCount == 1)
                    SetSlot(exploreToken2, token);

                exploreCount++;
            }
            else if (token.source == TokenSource.NPC)
            {
                SetSlot(npcToken, token);
            }
        }
    }

    private void SetSlot(Image slot, TokenDataSO token)
    {
        if (slot == null)
            return;

        slot.sprite = token.tokenIcon;
        slot.enabled = true;
        slot.color = Color.white;
    }

    private void ClearSlot(Image slot)
    {
        if (slot == null)
            return;

        slot.enabled = false;
        slot.sprite = null;
    }
}
