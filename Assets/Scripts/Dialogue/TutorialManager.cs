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

    [Header("Debug")]
    public bool enableTutorial = true;

    [Header("Dialogue")]
    public DialogueManager dialogue;

    public DialogueManager.Line[] introLines;
    public DialogueManager.Line[] templeLines;
    public DialogueManager.Line[] beforeRoyalLines;
    public DialogueManager.Line[] royalLines;

    [Header("Battle Start Dialogue")]
    public DialogueManager.Line[] battleStartLines;

    [Header("First Card Tutorial")]
    public DialogueManager.Line[] firstCardLines;

    [Header("Battle Result Dialogue")]
    public DialogueManager.Line[] winLines;
    public DialogueManager.Line[] loseLines;

    [Header("Buttons")]
    public GameObject templeButton;
    public GameObject royalButton;
    public GameObject dungeonButton;
    public GameObject shopButton;

    bool templeDialogueDone = false;
    public bool hasPlayedTraining = false;
    bool hasShownFirstCardTutorial = false;
    bool isPlayingBattleResultDialogue = false;
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
            return;
        }

        PlayerPrefs.DeleteKey("TutorialDone");

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
        if (templeButton != null) templeButton.SetActive(true);
        if (royalButton != null) royalButton.SetActive(true);
        if (dungeonButton != null) dungeonButton.SetActive(true);
        if (shopButton != null) shopButton.SetActive(true);
    }

    void StartIntro()
    {
        step = TutorialStep.Start;
        dialogue.StartDialogue(introLines);
    }

    public void OnDialogueEnd()
    {
        if (isPlayingBattleResultDialogue && !isTutorialFinished)
        {
            EndTutorial();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Town");

            return;
        }

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

    public void StartBattleStartTutorial(System.Action onEnd)
    {
        if (!enableTutorial || !BattleData.isTutorialBattle || step == TutorialStep.Done)
        {
            onEnd?.Invoke();
            return;
        }

        if (dialogue == null)
        {
            onEnd?.Invoke();
            return;
        }

        dialogue.StartDialogue(battleStartLines);

        onEnd?.Invoke();
    }

    public void PlayBattleResultDialogue(bool isWin)
    {
        if (dialogue == null) return;

        isPlayingBattleResultDialogue = true;

        if (isWin)
            dialogue.StartDialogue(winLines);
        else
            dialogue.StartDialogue(loseLines);
    }

    void OnEnable()
    {
        EntityManager.OnEntitySpawned += OnEntitySpawned;
    }

    void OnDisable()
    {
        EntityManager.OnEntitySpawned -= OnEntitySpawned;
    }

    IEnumerator CoFirstCardTutorial()
    {
        yield return new WaitForSeconds(0.3f);

        dialogue.StartDialogue(firstCardLines);
    }

    void OnEntitySpawned(bool isMine)
    {
        if (!enableTutorial) return;
        if (!isMine) return;
        if (!BattleData.isTutorialBattle) return;
        if (hasShownFirstCardTutorial) return;

        hasShownFirstCardTutorial = true;

        StartCoroutine(CoFirstCardTutorial());
    }

    public void EndTutorial()
    {
        isTutorialFinished = true;

        PlayerPrefs.SetInt("TutorialDone", 1);
        PlayerPrefs.Save();

        if (BattleData.tutorialEnemyDeck != null)
        {
            foreach (var card in BattleData.tutorialEnemyDeck.deckItems)
            {
                CardPool.Inst.AddCard(card);
            }
        }


        BattleData.isTutorialBattle = false;
        BattleData.tutorialEnemyDeck = null;

        step = TutorialStep.Done;

    }
}