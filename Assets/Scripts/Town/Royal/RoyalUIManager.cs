
using UnityEngine;

public class RoyalUIManager : MonoBehaviour
{
    public static RoyalUIManager Inst;

    public GameObject royalPanel;

    private bool isRoyalBGMActive = false;

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

            // Royal BGM ½ÃÀÛ
            if (!isRoyalBGMActive && AudioManager.Inst != null)
            {
                AudioManager.Inst.OpenRoyalBGM();
                isRoyalBGMActive = true;
            }

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

            if (isRoyalBGMActive)
            {
                AudioManager.Inst?.ClosePanelBGM();
                isRoyalBGMActive = false;
            }

            if (TutorialManager.Inst != null)
            {
                TutorialManager.Inst.CloseRoyalTutorialPanel();
                TutorialManager.Inst.ResumeTownIndicator();
            }
        });
    }
}
