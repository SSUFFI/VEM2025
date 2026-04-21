using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonUIManager : MonoBehaviour
{
    public static DungeonUIManager Inst;

    public GameObject dungeonPanel;

    [SerializeField] DeckSO trainingDeck;
    [SerializeField] UnityEngine.UI.Button dungeonEnterButton;

    void Awake()
    {
        Inst = this;
        dungeonPanel.SetActive(false);
    }

    public void OpenDungeonPanel()
    {
        dungeonPanel.SetActive(true);

        if (TutorialManager.Inst != null && !TutorialManager.Inst.hasPlayedTraining)
        {
            dungeonEnterButton.interactable = false;
        }
        else
        {
            dungeonEnterButton.interactable = true;
        }
    }

    public void CloseDungeonPanel()
    {
        dungeonPanel.SetActive(false);
    }

    public void OnClickTraining()
    {
        BattleData.isTutorialBattle = true;
        BattleData.tutorialEnemyDeck = trainingDeck;

        if (TutorialManager.Inst != null)
            TutorialManager.Inst.hasPlayedTraining = true;

        SceneManager.LoadScene("Battle");
    }

    public void OnClickDungeonEnter()
    {
        SceneManager.LoadScene("Map");
    }
}