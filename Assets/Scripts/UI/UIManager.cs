using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst;

    [Header("슬라이드 패널")]
    [SerializeField] TownSlidePanel inventorySlidePanel;
    [SerializeField] TownSlidePanel questSlidePanel;

    [Header("ESC로 닫을 일반 패널")]
    [SerializeField] List<GameObject> escapePanels = new List<GameObject>();

    [Header("씬별 ESC 메뉴 패널")]
    [SerializeField] GameObject townMenuPanel;
    [SerializeField] GameObject mapMenuPanel;
    [SerializeField] GameObject battleMenuPanel;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();
    }

    void HandleEscape()
    {
        if (questSlidePanel != null && questSlidePanel.IsOpen)
        {
            questSlidePanel.Close();
            return;
        }

        if (inventorySlidePanel != null && inventorySlidePanel.IsOpen)
        {
            inventorySlidePanel.Close();
            return;
        }

        for (int i = escapePanels.Count - 1; i >= 0; i--)
        {
            GameObject panel = escapePanels[i];

            if (panel != null && panel.activeInHierarchy)
            {
                panel.SetActive(false);
                return;
            }
        }

        ToggleSceneMenu();
    }

    void ToggleSceneMenu()
    {
        string scene = SceneManager.GetActiveScene().name;

        GameObject menu = null;

        switch (scene)
        {
            case "Town":
                menu = townMenuPanel;
                break;

            case "Map":
                menu = mapMenuPanel;
                break;

            case "Battle":
                menu = battleMenuPanel;
                break;
        }

        if (menu == null)
            return;

        menu.SetActive(!menu.activeSelf);
    }

    void CloseCurrentMenu()
    {
        string scene = SceneManager.GetActiveScene().name;

        GameObject menu = null;

        switch (scene)
        {
            case "Town":
                menu = townMenuPanel;
                break;

            case "Map":
                menu = mapMenuPanel;
                break;

            case "Battle":
                menu = battleMenuPanel;
                break;
        }

        if (menu != null)
            menu.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    public void Resume()
    {
        CloseCurrentMenu();
    }

    public void OnClickMenuButton()
    {
        ToggleSceneMenu();
    }

    public void OnClickSurrender()
    {
        SceneManager.LoadScene("Town");
    }

    public void GoTitle()
    {
        StartCoroutine(GoTitleCo());
    }

    IEnumerator GoTitleCo()
    {
        Time.timeScale = 1f;

        PlayerPrefs.Save();

        if (InventoryManager.Inst != null)
            Destroy(InventoryManager.Inst.gameObject);

        if (CardPool.Inst != null)
            Destroy(CardPool.Inst.gameObject);

        if (DeckEditManager.Inst != null)
            Destroy(DeckEditManager.Inst.gameObject);

        if (PlayerRelicManager.Inst != null)
            Destroy(PlayerRelicManager.Inst.gameObject);

        if (TutorialManager.Inst != null)
            Destroy(TutorialManager.Inst.gameObject);

        DialogueManager dialogue =
            FindObjectOfType<DialogueManager>();

        if (dialogue != null)
            Destroy(dialogue.gameObject);

        if (AccountManager.Inst != null)
            Destroy(AccountManager.Inst.gameObject);

        BattleData.isTutorialBattle = false;
        BattleData.tutorialEnemyDeck = null;
        BattleData.selectedEnemyDeck = null;

        yield return null;

        SceneManager.LoadScene("Title");
    }
}