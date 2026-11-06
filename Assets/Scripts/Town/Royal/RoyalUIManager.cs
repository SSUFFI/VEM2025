
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
        if (FadeManager.Instance.IsTransitioning)
            return;

        FadeManager.Instance.FadeTransition(() =>
        {
            if (TutorialManager.Inst != null)
                TutorialManager.Inst.SuspendTownIndicator();

            royalPanel.SetActive(true);

            if (TutorialManager.Inst != null)
                TutorialManager.Inst.OnRoyalEntered();

            DeckEditManager.Inst.LoadDeck();
            DeckListUI.Inst.Refresh();
        });
    }

    public void CloseRoyal()
    {
        if (FadeManager.Instance.IsTransitioning)
            return;

        FadeManager.Instance.FadeTransition(() =>
        {
            royalPanel.SetActive(false);

            if (TutorialManager.Inst != null)
            {
                TutorialManager.Inst.CloseRoyalTutorialPanel();
                TutorialManager.Inst.ResumeTownIndicator();
            }
        });
    }
}
