using UnityEngine;

public enum TutorialStep
{
    Start,
    AfterIntro,
    AfterTemple,
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
    public DialogueManager.Line[] royalLines;

    [Header("Buttons")]
    public GameObject templeButton;
    public GameObject royalButton;
    public GameObject dungeonButton;
    public GameObject shopButton;

    bool templeDialogueDone = false;

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

            case TutorialStep.AfterTemple:
                royalButton.SetActive(true);
                step = TutorialStep.AfterRelic;
                break;

            case TutorialStep.AfterRoyal:
                step = TutorialStep.AfterDeck;
                break;
        }
    }

    public void OnTempleEntered()
    {
        if (step == TutorialStep.AfterIntro && !templeDialogueDone)
        {
            templeDialogueDone = true;
            dialogue.StartDialogue(templeLines);
            return;
        }

        TempleUIManager.Inst.OpenMainPanel();
    }

    public void OnRoyalEntered()
    {
        if (step == TutorialStep.AfterRelic)
        {
            dialogue.StartDialogue(royalLines);
            step = TutorialStep.AfterRoyal;
        }
    }

    public void OnDeckCompleted()
    {
        if (step == TutorialStep.AfterRoyal)
        {
            dungeonButton.SetActive(true);
            step = TutorialStep.AfterDeck;
        }
    }

    public void EndTutorial()
    {
        PlayerPrefs.SetInt("TutorialDone", 1);
        PlayerPrefs.Save();
        templeDialogueDone = true;
        step = TutorialStep.Done;
    }
}