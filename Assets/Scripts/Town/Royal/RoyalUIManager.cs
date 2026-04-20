using UnityEngine;

public class RoyalUIManager : MonoBehaviour
{
    public static RoyalUIManager Inst;

    public GameObject royalPanel;


    void Awake()
    {
        Inst = this;
    }


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