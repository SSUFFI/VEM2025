using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonUIManager : MonoBehaviour
{
    public static DungeonUIManager Inst;

    public GameObject dungeonPanel;

    void Awake()
    {
        Inst = this;
        dungeonPanel.SetActive(false);
    }

    public void OpenDungeonPanel()
    {
        dungeonPanel.SetActive(true);
    }

    public void CloseDungeonPanel()
    {
        dungeonPanel.SetActive(false);
    }

    public void OnClickTraining()
    {
        SceneManager.LoadScene("Battle");
    }

    public void OnClickDungeonEnter()
    {
        SceneManager.LoadScene("Map");
    }
}