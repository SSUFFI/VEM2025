using UnityEngine;
using System.Collections;

public enum TutorialStep
{
    Start,
    AfterIntro,
    AfterTemple,
    BeforeRoyal,
    AfterRelic,
    AfterRoyal,
    AfterDeck,
    AfterDungeon,
    Done
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Inst;

    public TutorialStep step;

    [Header("Dialogue")]
    public DialogueManager dialogue;

    public DialogueManager.Line[] introLines;
    public DialogueManager.Line[] templeLines;
    public DialogueManager.Line[] beforeRoyalLines;
    public DialogueManager.Line[] royalLines;

    [Header("Buttons")]
    public GameObject templeButton;
    public GameObject royalButton;
    public GameObject dungeonButton;
    public GameObject shopButton;

    bool templeDialogueDone = false;

    DialogueManager.Line[] resumeLines;
    int resumeIndex = -1;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        {
            UnlockAll();
            step = TutorialStep.Done;
            return;
        }

        LockAll();
        StartIntro();
    }

    void LockAll()
    {
        templeButton.SetActive(false);
        royalButton.SetActive(false);
        dungeonButton.SetActive(false);
        shopButton.SetActive(false);
    }

    void UnlockAll()
    {
        templeButton.SetActive(true);
        royalButton.SetActive(true);
        dungeonButton.SetActive(true);
        shopButton.SetActive(true);
    }

    void StartIntro()
    {
        step = TutorialStep.Start;
        dialogue.StartDialogue(introLines);
    }


    public void OnDialogueEnd()
    {
        switch (step)
        {
            case TutorialStep.Start:
                templeButton.SetActive(true);
                step = TutorialStep.AfterIntro;
                break;

            case TutorialStep.AfterIntro:
                step = TutorialStep.AfterTemple;
                break;

            case TutorialStep.BeforeRoyal:
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

            StartCoroutine(CoPlayTempleDialogue());
        }
    }

    public void OnTempleExit()
    {
        if (step == TutorialStep.AfterTemple)
        {
            dialogue.StartDialogue(beforeRoyalLines);
            step = TutorialStep.BeforeRoyal;
        }
    }

    IEnumerator CoPlayTempleDialogue()
    {
        yield return new WaitForSeconds(0.2f);

        dialogue.StartDialogue(templeLines);
    }

    public void OnRoyalEntered()
    {
        if (RoyalUIManager.Inst != null)
            RoyalUIManager.Inst.OpenRoyal();

        if (step == TutorialStep.BeforeRoyal)
        {
            dialogue.StartDialogue(royalLines);

            resumeLines = royalLines;
            resumeIndex = 8;

            step = TutorialStep.AfterRoyal;
        }
    }


    public void OnDeckCompleted()
    {
        if (step == TutorialStep.AfterRoyal)
        {
            ResumeDialogue();

            dungeonButton.SetActive(true);
            step = TutorialStep.AfterDeck;
        }
    }

    void ResumeDialogue()
    {
        if (resumeLines == null) return;

        dialogue.StartDialogue(resumeLines);
        dialogue.SetIndex(resumeIndex);

        resumeLines = null;
        resumeIndex = -1;
    }

    public void EndTutorial()
    {
        PlayerPrefs.SetInt("TutorialDone", 1);
        PlayerPrefs.Save();

        templeDialogueDone = true;
        step = TutorialStep.Done;
    }
}