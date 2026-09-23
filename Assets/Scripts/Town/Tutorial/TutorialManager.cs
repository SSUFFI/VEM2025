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

    [Header("Tutorial Reward Cards")]
    [SerializeField]
    List<CardDataSO> tutorialRewardCards =
        new List<CardDataSO>();

    bool templeDialogueDone = false;
    bool isTutorialFinished = false;

    DialogueManager.Line[] resumeLines;
    int resumeIndex = -1;

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
    }

    void StartIntro()
    {
        step = TutorialStep.Start;

        if (dialogue != null)
            dialogue.StartDialogue(introLines);
    }

    public void OnDialogueEnd()
    {
        switch (step)
        {
            case TutorialStep.Start:

                if (templeButton != null)
                    templeButton.SetActive(true);

                step = TutorialStep.AfterIntro;
                break;


            case TutorialStep.AfterIntro:

                step = TutorialStep.AfterTemple;
                break;


            case TutorialStep.BeforeRoyal:

                if (royalButton != null)
                    royalButton.SetActive(true);

                break;


            case TutorialStep.AfterRoyal:
                break;
        }
    }

    public void OnTempleEntered()
    {
        if (!templeDialogueDone)
        {
            templeDialogueDone = true;

            StartCoroutine(
                CoPlayTempleDialogue());
        }
    }

    public void OnTempleExit()
    {
        if (step == TutorialStep.AfterTemple)
        {
            if (dialogue != null)
                dialogue.StartDialogue(
                    beforeRoyalLines);

            step = TutorialStep.BeforeRoyal;
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
        if (step == TutorialStep.BeforeRoyal)
        {
            if (dialogue != null)
                dialogue.StartDialogue(royalLines);

            resumeLines = royalLines;
            resumeIndex = 9;

            step = TutorialStep.AfterRoyal;
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
            step = TutorialStep.Training;
        }
    }

    public void CompleteNewTrainingTutorial()
    {
        if (isTutorialFinished)
            return;

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