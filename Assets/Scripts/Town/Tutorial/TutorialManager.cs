using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TutorialStep
{
    Start,
    AfterIntro,
    AfterTemple,
    BeforeRoyal,
    AfterRoyal,
    AfterDeck,
    Training,
    Done
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Inst;

    public TutorialStep step;

    [Header("Debug")]
    public bool enableTutorial = true;

    [Header("Temple Tutorial")]
    [SerializeField] GameObject templeTutorialPanel;

    [Header("Temple Tutorial Hidden UI")]
    [SerializeField] GameObject[] templeHiddenUI;

    [Header("Royal Tutorial")]
    [SerializeField] GameObject royalTutorialPanel;

    [Header("Royal Tutorial Hidden UI")]
    [SerializeField] GameObject[] royalHiddenUI;

    [Header("Tutorial Indicator Targets")]
    [SerializeField] RectTransform templeIndicatorTarget;
    [SerializeField] RectTransform priestIndicatorTarget;
    [SerializeField] RectTransform royalIndicatorTarget;
    [SerializeField] RectTransform kingIndicatorTarget;
    [SerializeField] RectTransform trainingIndicatorTarget;

    [Header("Tutorial Indicator Settings")]
    [SerializeField] float indicatorOffsetY = 40f;
    [SerializeField] float indicatorScale = 1f;

    [Header("Dialogue")]
    public DialogueManager dialogue;

    public DialogueManager.Line[] introLines;
    public DialogueManager.Line[] templeLines;
    public DialogueManager.Line[] beforeRoyalLines;
    public DialogueManager.Line[] royalLines;

    [Header("Buttons")]
    public GameObject templeButton;
    public GameObject royalButton;
    public GameObject trainingButton;
    public GameObject dungeonButton;
    public GameObject shopButton;
    public GameObject inventoryButton;
    public GameObject questButton;

    [Header("Tutorial Reward Cards")]
    [SerializeField]
    List<CardDataSO> tutorialRewardCards =
        new List<CardDataSO>();

    bool templeDialogueDone = false;
    bool isTutorialFinished = false;
    bool isTownIndicatorSuspended = false;

    DialogueManager.Line[] resumeLines;
    int resumeIndex = -1;

    bool isRoyalFirstDialogue = false;


    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogue == null)
            dialogue = FindObjectOfType<DialogueManager>();
    }


    void Start()
    {
        if (!enableTutorial)
        {
            UnlockAll();
            step = TutorialStep.Done;
            isTutorialFinished = true;
            return;
        }

        string tutorialKey =
            AccountManager.GetAccountKey("TutorialDone");

        if (PlayerPrefs.GetInt(tutorialKey, 0) == 1)
        {
            UnlockAll();

            isTutorialFinished = true;
            step = TutorialStep.Done;

            return;
        }

        LockAll();
        StartIntro();
    }

    void LockAll()
    {
        if (templeButton != null)
            templeButton.SetActive(false);

        if (royalButton != null)
            royalButton.SetActive(false);

        if (trainingButton != null)
            trainingButton.SetActive(false);

        if (dungeonButton != null)
            dungeonButton.SetActive(false);

        if (shopButton != null)
            shopButton.SetActive(false);

        if (inventoryButton != null)
            inventoryButton.SetActive(false);

        if (questButton != null)
            questButton.SetActive(false);
    }

    void UnlockAll()
    {
        if (templeButton != null)
            templeButton.SetActive(true);

        if (royalButton != null)
            royalButton.SetActive(true);

        if (trainingButton != null)
            trainingButton.SetActive(true);

        if (dungeonButton != null)
            dungeonButton.SetActive(true);

        if (shopButton != null)
            shopButton.SetActive(true);

        if (inventoryButton != null)
            inventoryButton.SetActive(true);

        if (questButton != null)
            questButton.SetActive(true);
    }

    public void RegisterTownButtons(
    GameObject temple,
    GameObject royal,
    GameObject training,
    GameObject dungeon,
    GameObject shop,
    GameObject inventory,
    GameObject quest)
    {
        templeButton = temple;
        royalButton = royal;
        trainingButton = training;
        dungeonButton = dungeon;
        shopButton = shop;
        inventoryButton = inventory;
        questButton = quest;

        RefreshTownButtons();
    }

    public void RefreshTownButtons()
    {
        if (!enableTutorial || isTutorialFinished || step == TutorialStep.Done)
        {
            UnlockAll();
            return;
        }

        if (templeButton != null)
            templeButton.SetActive(step != TutorialStep.Start);

        if (royalButton != null)
            royalButton.SetActive(
                step == TutorialStep.BeforeRoyal ||
                step == TutorialStep.AfterRoyal ||
                step == TutorialStep.AfterDeck ||
                step == TutorialStep.Training);

        if (trainingButton != null)
            trainingButton.SetActive(
                step == TutorialStep.AfterDeck ||
                step == TutorialStep.Training);

        if (dungeonButton != null)
            dungeonButton.SetActive(false);

        if (shopButton != null)
            shopButton.SetActive(false);

        if (inventoryButton != null)
            inventoryButton.SetActive(false);

        if (questButton != null)
            questButton.SetActive(false);
    }

    void ShowTutorialIndicator(RectTransform target)
    {
        if (TutorialIndicatorManager.Inst == null || target == null)
            return;

        TutorialIndicatorManager.Inst.Show(
            target,
            indicatorOffsetY,
            indicatorScale);
    }

    public void SuspendTownIndicator()
    {
        if (TutorialIndicatorManager.Inst == null)
            return;

        isTownIndicatorSuspended = true;
        TutorialIndicatorManager.Inst.Suspend();
    }

    public void ResumeTownIndicator()
    {
        if (!isTownIndicatorSuspended)
            return;

        isTownIndicatorSuspended = false;

        if (TutorialIndicatorManager.Inst != null)
            TutorialIndicatorManager.Inst.Resume();
    }

    void HideTutorialIndicator()
    {
        if (TutorialIndicatorManager.Inst != null)
            TutorialIndicatorManager.Inst.Hide();
    }

    void StartIntro()
    {
        step = TutorialStep.Start;

        if (dialogue != null)
            dialogue.StartDialogue(introLines);
    }

    public void OnDialogueEnd()
    {
        if (isRoyalFirstDialogue)
        {
            isRoyalFirstDialogue = false;
            SetRoyalUIVisible(true);
        }

        switch (step)
        {
            case TutorialStep.Start:

                if (templeButton != null)
                    templeButton.SetActive(true);

                step = TutorialStep.AfterIntro;

                ShowTutorialIndicator(templeIndicatorTarget);
                break;

            case TutorialStep.AfterIntro:

                SetTempleUIVisible(true);

                step = TutorialStep.AfterTemple;
                break;

            case TutorialStep.BeforeRoyal:

                if (royalButton != null)
                    royalButton.SetActive(true);

                ShowTutorialIndicator(royalIndicatorTarget);
                break;

            case TutorialStep.AfterRoyal:
                break;

            case TutorialStep.AfterDeck:
                break;
        }
    }

    public void OnTempleEntered()
    {
        if (!enableTutorial || isTutorialFinished || templeDialogueDone)
            return;

        HideTutorialIndicator();

        if (templeTutorialPanel != null)
        {
            templeTutorialPanel.SetActive(true);
            ShowTutorialIndicator(priestIndicatorTarget);
        }
        else
        {
            Debug.LogWarning("성당 튜토리얼 패널이 연결되지 않았습니다.");
        }
    }

    public void OnTemplePriestClicked()
    {
        if (!enableTutorial || isTutorialFinished || templeDialogueDone)
            return;

        templeDialogueDone = true;

        HideTutorialIndicator();

        if (templeTutorialPanel != null)
            templeTutorialPanel.SetActive(false);

        SetTempleUIVisible(false);

        StartCoroutine(CoPlayTempleDialogue());
    }

    public void OnTempleExit()
    {
        if (templeTutorialPanel != null)
            templeTutorialPanel.SetActive(false);

        if (step == TutorialStep.AfterIntro && !templeDialogueDone)
        {
            ShowTutorialIndicator(templeIndicatorTarget);
            return;
        }

        if (step == TutorialStep.AfterTemple && templeDialogueDone)
        {
            HideTutorialIndicator();

            if (dialogue != null)
                dialogue.StartDialogue(beforeRoyalLines);

            step = TutorialStep.BeforeRoyal;
        }
    }

    void SetTempleUIVisible(bool visible)
    {
        foreach (GameObject obj in templeHiddenUI)
        {
            if (obj != null)
                obj.SetActive(visible);
        }
    }

    IEnumerator CoPlayTempleDialogue()
    {
        yield return new WaitForSeconds(0.2f);

        if (dialogue != null)
            dialogue.StartDialogue(templeLines);
    }

    public void OnRoyalEntered()
    {
        if (step != TutorialStep.BeforeRoyal)
            return;

        HideTutorialIndicator();

        if (royalTutorialPanel != null)
        {
            royalTutorialPanel.SetActive(true);
            ShowTutorialIndicator(kingIndicatorTarget);
        }
        else
        {
            Debug.LogWarning("왕국 튜토리얼 패널이 연결되지 않았습니다.");
        }
    }

    public void OnRoyalKingClicked()
    {
        if (step != TutorialStep.BeforeRoyal)
            return;

        HideTutorialIndicator();

        if (royalTutorialPanel != null)
            royalTutorialPanel.SetActive(false);

        isRoyalFirstDialogue = true;
        SetRoyalUIVisible(false);

        resumeLines = royalLines;
        resumeIndex = 9;

        step = TutorialStep.AfterRoyal;

        if (dialogue != null)
            dialogue.StartDialogue(royalLines);
    }

    public void CloseRoyalTutorialPanel()
    {
        if (royalTutorialPanel != null)
            royalTutorialPanel.SetActive(false);

        if (step == TutorialStep.BeforeRoyal)
            ShowTutorialIndicator(royalIndicatorTarget);

        if (step == TutorialStep.AfterDeck && dialogue != null && !dialogue.IsDialoguePlaying)
            ShowTutorialIndicator(trainingIndicatorTarget);
    }

    void SetRoyalUIVisible(bool visible)
    {
        foreach (GameObject obj in royalHiddenUI)
        {
            if (obj != null)
                obj.SetActive(visible);
        }
    }

    public void OnDeckCompleted()
    {
        if (step != TutorialStep.AfterRoyal)
            return;

        ResumeDialogue();

        if (trainingButton != null)
            trainingButton.SetActive(true);

        if (dungeonButton != null)
            dungeonButton.SetActive(false);

        step = TutorialStep.AfterDeck;
    }

    void ResumeDialogue()
    {
        if (resumeLines == null)
            return;

        if (dialogue == null)
            return;

        dialogue.StartDialogue(resumeLines);
        dialogue.SetIndex(resumeIndex);

        resumeLines = null;
        resumeIndex = -1;
    }

    public void OnTrainingEntered()
    {
        if (step == TutorialStep.AfterDeck)
        {
            HideTutorialIndicator();
            step = TutorialStep.Training;
        }
    }

    public void CompleteNewTrainingTutorial()
    {
        if (isTutorialFinished)
            return;

        HideTutorialIndicator();
        GiveTutorialRewardCards();

        isTutorialFinished = true;
        step = TutorialStep.Done;

        string tutorialKey =
            AccountManager.GetAccountKey(
                "TutorialDone");

        PlayerPrefs.SetInt(
            tutorialKey,
            1);

        PlayerPrefs.Save();

        UnlockAll();

        if (TrainingSelectManager.Inst != null)
        {
            TrainingSelectManager.Inst
                .RefreshButtons();
        }

        Debug.Log(
            "새 전투 튜토리얼 완료 / 카드 보상 지급 / 미궁 해금");
    }

    void GiveTutorialRewardCards()
    {
        if (CardPool.Inst == null)
        {
            Debug.LogWarning(
                "CardPool이 없어 튜토리얼 보상을 지급할 수 없습니다.");

            return;
        }

        foreach (CardDataSO card in tutorialRewardCards)
        {
            if (card == null)
                continue;

            CardPool.Inst.AddCard(card);
        }
    }

    public bool IsTutorialFinished()
    {
        return isTutorialFinished;
    }
}