using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingSelectManager : MonoBehaviour
{
    public static TrainingSelectManager Inst;

    [Header("Panel")]
    [SerializeField] GameObject trainingPanel;

    [Header("Training Buttons")]
    [SerializeField] Button training1Button;
    [SerializeField] Button training2Button;
    [SerializeField] Button training3Button;
    [SerializeField] Button mockBattleButton;

    [Header("Mock Battle")]
    [SerializeField] DeckSO mockBattleDeck;

    static bool openPanelOnReturn;

    const string TRAINING_1_KEY = "Training1Completed";
    const string TRAINING_2_KEY = "Training2Completed";
    const string TRAINING_3_KEY = "Training3Completed";

    void Awake()
    {
        Inst = this;

        if (trainingPanel != null)
            trainingPanel.SetActive(false);
    }

    void Start()
    {
        if (openPanelOnReturn)
        {
            openPanelOnReturn = false;
            OpenTrainingPanel();
        }
    }

    public static void OpenPanelAfterReturn()
    {
        openPanelOnReturn = true;
    }

    public void OpenTrainingPanel()
    {
        if (trainingPanel == null)
            return;

        RefreshButtons();
        trainingPanel.SetActive(true);
    }

    public void CloseTrainingPanel()
    {
        if (trainingPanel == null)
            return;

        trainingPanel.SetActive(false);
    }

    public void RefreshButtons()
    {
        bool training1Completed =
            IsTraining1Completed;

        bool training2Completed =
            IsTraining2Completed;

        bool training3Completed =
            IsTraining3Completed;

        SetButtonState(
            training1Button,
            true);

        SetButtonState(
            training2Button,
            training1Completed);

        SetButtonState(
            training3Button,
            training2Completed);

        SetButtonState(
            mockBattleButton,
            training3Completed);
    }

    void SetButtonState(
        Button button,
        bool unlocked)
    {
        if (button == null)
            return;

        button.interactable = unlocked;

        Color color = unlocked
            ? Color.white
            : new Color(
                0.45f,
                0.45f,
                0.45f,
                1f);

        Graphic[] graphics =
            button.GetComponentsInChildren<Graphic>(
                true);

        foreach (Graphic graphic in graphics)
        {
            graphic.color = color;
        }
    }

    public void StartTraining1()
    {
        BattleData.SetBattleMode(
            BattleMode.Tutorial1);

        BattleData.selectedEnemyDeck = null;
        BattleData.tutorialEnemyDeck = null;

        SceneManager.LoadScene("Battle");
    }

    public void StartTraining2()
    {
        if (!IsTraining1Completed)
            return;

        BattleData.SetBattleMode(
            BattleMode.Tutorial2);

        BattleData.selectedEnemyDeck = null;
        BattleData.tutorialEnemyDeck = null;

        SceneManager.LoadScene("Battle");
    }

    public void StartTraining3()
    {
        if (!IsTraining2Completed)
            return;

        BattleData.SetBattleMode(
            BattleMode.Tutorial3);

        BattleData.selectedEnemyDeck = null;
        BattleData.tutorialEnemyDeck = null;

        SceneManager.LoadScene("Battle");
    }

    public void StartMockBattle()
    {
        if (!IsTraining3Completed)
            return;

        if (mockBattleDeck == null)
        {
            Debug.LogWarning(
                "TrainingSelectManager에 Mock Battle Deck이 연결되지 않았습니다.");

            return;
        }

        BattleData.ResetBattleMode();

        BattleData.isTutorialBattle = true;
        BattleData.tutorialEnemyDeck = mockBattleDeck;

        SceneManager.LoadScene("Battle");
    }

    public static void CompleteTraining1()
    {
        SetCompleted(
            TRAINING_1_KEY);

        if (Inst != null)
            Inst.RefreshButtons();
    }

    public static void CompleteTraining2()
    {
        SetCompleted(
            TRAINING_2_KEY);

        if (Inst != null)
            Inst.RefreshButtons();
    }

    public static void CompleteTraining3()
    {
        bool wasAlreadyCompleted =
            IsTraining3Completed;

        SetCompleted(
            TRAINING_3_KEY);

        if (Inst != null)
            Inst.RefreshButtons();

        if (!wasAlreadyCompleted &&
            TutorialManager.Inst != null)
        {
            TutorialManager.Inst.CompleteNewTrainingTutorial();
        }
    }

    static void SetCompleted(string key)
    {
        PlayerPrefs.SetInt(
            AccountManager.GetAccountKey(key),
            1);

        PlayerPrefs.Save();
    }

    static bool GetCompleted(string key)
    {
        return PlayerPrefs.GetInt(
            AccountManager.GetAccountKey(key),
            0) == 1;
    }

    public static bool IsTraining1Completed =>
        GetCompleted(TRAINING_1_KEY);

    public static bool IsTraining2Completed =>
        GetCompleted(TRAINING_2_KEY);

    public static bool IsTraining3Completed =>
        GetCompleted(TRAINING_3_KEY);

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}