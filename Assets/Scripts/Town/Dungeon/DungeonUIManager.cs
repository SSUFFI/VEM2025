using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUIManager : MonoBehaviour
{
    public static DungeonUIManager Inst;

    public GameObject dungeonPanel;

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

        if (stage1Button != null)
        {
            stage1Button.interactable = tutorialClear;
        }

        if (stage2Button != null)
        {
            stage2Button.interactable =
                tutorialClear &&
                StageProgress.highestClearedStage >= 1;
        }

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

    public void OnClickStage1()
    {
        EnterStage(1);
    }

    public void OnClickStage2()
    {
        EnterStage(2);
    }

    public void OnClickStage3()
    {
        EnterStage(3);
    }

    void EnterStage(int stage)
    {
        if (!IsTutorialCompleted())
            return;

        if (FadeManager.Instance != null &&
            FadeManager.Instance.IsTransitioning)
            return;

        StageProgress.selectedStage = stage;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.ChangeScene("Map");
        }
        else
        {
            SceneManager.LoadScene("Map");
        }
    }

    bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(
            AccountManager.GetAccountKey(
                "TutorialDone"),
            0) == 1;
    }
}