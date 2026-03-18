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

    // --------------------------
    // 버튼 연결용 함수들
    // --------------------------

    // 대화 끝나고 호출
    public void OpenMainPanel()
    {
        OpenPanel(mainPanel);
    }

    // 1. 성유물
    public void OnClickRelic()
    {
        OpenPanel(relicPanel);
    }

    // 성유물 → 강화 패널
    public void OnClickRelicEnhance()
    {
        OpenPanel(relicEnhancePanel);
    }

    // 강화 패널 → 성유물 패널
    public void OnClickBackToRelic()
    {
        OpenPanel(relicPanel);
    }

    // 2. 쪽지
    public void OnClickNote()
    {
        OpenPanel(notePanel);
    }

    // 3. 마을로 돌아가기
    public void OnClickExit()
    {
        CloseTemple();
    }

    // 돌아가기 버튼 (공통)
    public void OnClickBackToMain()
    {
        OpenPanel(mainPanel);
    }
}