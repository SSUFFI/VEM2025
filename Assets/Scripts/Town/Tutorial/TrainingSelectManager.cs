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

    static bool training1Completed;
    static bool training2Completed;
    static bool training3Completed;
    static bool openPanelOnReturn;

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
            true);
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
        if (!training1Completed)
            return;

        BattleData.SetBattleMode(BattleMode.Tutorial2);

        BattleData.selectedEnemyDeck = null;
        BattleData.tutorialEnemyDeck = null;

        SceneManager.LoadScene("Battle");
    }

    public void StartTraining3()
    {
        if (!training2Completed)
            return;

        BattleData.SetBattleMode(BattleMode.Tutorial3);

        BattleData.selectedEnemyDeck = null;
        BattleData.tutorialEnemyDeck = null;

        SceneManager.LoadScene("Battle");
    }

    public static void CompleteTraining1()
    {
        training1Completed = true;

        if (Inst != null)
            Inst.RefreshButtons();
    }

    public static void CompleteTraining2()
    {
        training2Completed = true;

        if (Inst != null)
            Inst.RefreshButtons();
    }

    public static void CompleteTraining3()
    {
        training3Completed = true;

        if (Inst != null)
            Inst.RefreshButtons();
    }

    public static bool IsTraining1Completed =>
        training1Completed;

    public static bool IsTraining2Completed =>
        training2Completed;

    public static bool IsTraining3Completed =>
        training3Completed;

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}