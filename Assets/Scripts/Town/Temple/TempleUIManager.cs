
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleUIManager : MonoBehaviour
{
    public static TempleUIManager Inst;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject relicPanel;
    public GameObject notePanel;
    public GameObject relicEnhancePanel;

    void Awake()
    {
        Inst = this;
        CloseAllPanels();
    }

    public void OpenPanel(GameObject panel)
    {
        CloseAllPanels();
        panel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        mainPanel.SetActive(false);
        relicPanel.SetActive(false);
        notePanel.SetActive(false);
        relicEnhancePanel.SetActive(false);
    }

    public void CloseTemple()
    {
        CloseAllPanels();
    }

    public void OpenMainPanel()
    {
        if (FadeManager.Instance != null &&
            FadeManager.Instance.IsTransitioning)
            return;

        if (FadeManager.Instance == null)
        {
            OpenMainPanelImmediate();
            return;
        }

        FadeManager.Instance.FadeTransition(() =>
        {
            OpenMainPanelImmediate();
        });
    }

    void OpenMainPanelImmediate()
    {
        if (TutorialManager.Inst != null)
            TutorialManager.Inst.SuspendTownIndicator();

        OpenPanel(mainPanel);

        if (TutorialManager.Inst != null &&
            TutorialManager.Inst.step != TutorialStep.Done)
        {
            TutorialManager.Inst.OnTempleEntered();
        }

        if (TutorialManager.Inst != null &&
            TutorialManager.Inst.EnableTutorial &&
            !TutorialManager.Inst.IsTutorialFinished() &&
            !TutorialManager.Inst.TempleDialogueDone)
            return;

        TempleDoorController.Inst?.OpenDoors();
    }

    public void OnClickRelic()
    {
        OpenPanel(relicPanel);
    }

    public void OnClickRelicEnhance()
    {
        OpenPanel(relicEnhancePanel);
    }

    public void OnClickBackToRelic()
    {
        OpenPanel(relicPanel);
    }

    public void OnClickNote()
    {
        OpenPanel(notePanel);
    }

    public void OnClickExit()
    {
        if (FadeManager.Instance == null)
        {
            ExitTempleImmediate();
            return;
        }

        FadeManager.Instance.FadeTransition(() =>
        {
            ExitTempleImmediate();
        });
    }

    void ExitTempleImmediate()
    {
        CloseTemple();

        if (TutorialManager.Inst != null)
        {
            TutorialManager.Inst.OnTempleExit();
            TutorialManager.Inst.ResumeTownIndicator();
        }
    }

    public void OnClickBackToMain()
    {
        OpenPanel(mainPanel);
    }
}
