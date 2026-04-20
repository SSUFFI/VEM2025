using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleUIManager : MonoBehaviour
{
    public static TempleUIManager Inst;

    void Awake()
    {
        Inst = this;
        CloseAllPanels();
    }

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject relicPanel;
    public GameObject notePanel;
    public GameObject relicEnhancePanel;


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
        OpenPanel(mainPanel);
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
        CloseTemple();

        if (TutorialManager.Inst != null)
            TutorialManager.Inst.OnTempleExit();
    }

    public void OnClickBackToMain()
    {
        OpenPanel(mainPanel);
    }
}