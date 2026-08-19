using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUIManager : MonoBehaviour
{
    public static DungeonUIManager Inst;

    public GameObject dungeonPanel;

    [SerializeField] DeckSO trainingDeck;

    [Header("Stage Buttons")]
    [SerializeField] Button stage1Button;
    [SerializeField] Button stage2Button;
    [SerializeField] Button stage3Button;

    void Awake()
    {
        Inst = this;

        if (dungeonPanel != null)
            dungeonPanel.SetActive(false);
    }

    public void OpenDungeonPanel()
    {
        if (dungeonPanel != null)
            dungeonPanel.SetActive(true);

        bool tutorialClear =
            PlayerPrefs.GetInt(
                AccountManager.GetAccountKey("TutorialDone"),
                0) == 1;

        // Stage 1
        if (stage1Button != null)
        {
            stage1Button.interactable =
                tutorialClear;
        }

        // Stage 2
        if (stage2Button != null)
        {
            stage2Button.interactable =
                tutorialClear &&
                StageProgress.highestClearedStage >= 1;
        }

        // Stage 3
        if (stage3Button != null)
        {
            stage3Button.interactable =
                tutorialClear &&
                StageProgress.highestClearedStage >= 2;
        }
    }

    public void CloseDungeonPanel()
    {
        if (dungeonPanel != null)
            dungeonPanel.SetActive(false);
    }

    public void OnClickTraining()
    {
        BattleData.isTutorialBattle = true;
        BattleData.tutorialEnemyDeck = trainingDeck;

        SceneManager.LoadScene("Battle");
    }

    public void OnClickStage1()
    {
        StageProgress.selectedStage = 1;

        SceneManager.LoadScene("Map");
    }

    public void OnClickStage2()
    {
        StageProgress.selectedStage = 2;

        SceneManager.LoadScene("Map");
    }

    public void OnClickStage3()
    {
        StageProgress.selectedStage = 3;

        SceneManager.LoadScene("Map");
    }
}