using UnityEngine;

public class RoyalUIManager : MonoBehaviour
{
    public GameObject royalPanel;

    public void OpenRoyal()
    {
        royalPanel.SetActive(true);

        DeckEditManager.Inst.LoadDeck();
        DeckListUI.Inst.Refresh();
    }

    public void CloseRoyal()
    {
        royalPanel.SetActive(false);
    }
}