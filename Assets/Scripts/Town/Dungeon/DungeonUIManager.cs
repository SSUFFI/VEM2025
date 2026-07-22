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
        dungeonPanel.SetActive(false);
    }

    public void OpenDungeonPanel()
    {
        dungeonPanel.SetActive(true);

        bool tutorialClear = TutorialManager.Inst == null ||
                             TutorialManager.Inst.hasFinishedTraining;

        stage1Button.interactable = tutorialClear;

        stage2Button.interactable =
            tutorialClear &&
            StageProgress.highestClearedStage >= 1;

        stage3Button.interactable =
            tutorialClear &&
            StageProgress.highestClearedStage >= 2;
    }

    public void CloseDungeonPanel()
    {
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