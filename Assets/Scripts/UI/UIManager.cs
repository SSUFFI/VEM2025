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

    [Header("마을 튜토리얼 버튼")]
    [SerializeField] GameObject templeButton;
    [SerializeField] GameObject royalButton;
    [SerializeField] GameObject trainingButton;
    [SerializeField] GameObject dungeonButton;
    [SerializeField] GameObject shopButton;
    [SerializeField] GameObject inventoryButton;
    [SerializeField] GameObject questButton;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Town")
            return;

        if (TutorialManager.Inst == null)
            return;

        TutorialManager.Inst.RegisterTownButtons(
            templeButton,
            royalButton,
            trainingButton,
            dungeonButton,
            shopButton,
            inventoryButton,
            questButton);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();
    }

    void HandleEscape()
    {
        if (FadeManager.Instance != null &&
            FadeManager.Instance.IsTransitioning)
            return;

        string scene = SceneManager.GetActiveScene().name;

        bool isTownTutorial =
            scene == "Town" &&
            TutorialManager.Inst != null &&
            TutorialManager.Inst.enableTutorial &&
            TutorialManager.Inst.step != TutorialStep.Done;

        if (isTownTutorial)
        {
            if (townMenuPanel != null)
            {
                bool willOpen = !townMenuPanel.activeSelf;

                townMenuPanel.SetActive(willOpen);

                if (willOpen)
                    townMenuPanel.transform.SetAsLastSibling();
            }

            return;
        }

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

            if (panel == null || !panel.activeInHierarchy)
                continue;

            if (ShopManager.Inst != null &&
                panel == ShopManager.Inst.ShopPanel)
            {
                ShopManager.Inst.CloseShop();
                return;
            }

            if (TrainingSelectManager.Inst != null &&
                panel == TrainingSelectManager.Inst.TrainingPanel)
            {
                TrainingSelectManager.Inst.CloseTrainingPanel();
                return;
            }

            if (TempleUIManager.Inst != null &&
                (panel == TempleUIManager.Inst.mainPanel ||
                 panel == TempleUIManager.Inst.relicPanel ||
                 panel == TempleUIManager.Inst.notePanel ||
                 panel == TempleUIManager.Inst.relicEnhancePanel))
            {
                TempleUIManager.Inst.OnClickExit();
                return;
            }

            if (RoyalUIManager.Inst != null &&
                panel == RoyalUIManager.Inst.royalPanel)
            {
                RoyalUIManager.Inst.CloseRoyal();
                return;
            }

            panel.SetActive(false);
            return;
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
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.ChangeScene("Town");
        }
        else
        {
            SceneManager.LoadScene("Town");
        }
    }

    public void GoTitle()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.ChangeScene(
                "Title",
                CleanupBeforeTitle);
        }
        else
        {
            StartCoroutine(GoTitleCo());
        }
    }

    IEnumerator GoTitleCo()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeTransition(() =>
            {
                CleanupBeforeTitle();
            });

            yield return new WaitUntil(() =>
                FadeManager.Instance == null ||
                !FadeManager.Instance.IsTransitioning);

            if (FadeManager.Instance != null)
            {
                FadeManager.Instance.ChangeScene("Title");
                yield break;
            }
        }
        else
        {
            CleanupBeforeTitle();
            yield return null;
            SceneManager.LoadScene("Title");
        }
    }

    void CleanupBeforeTitle()
    {
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
    }
}