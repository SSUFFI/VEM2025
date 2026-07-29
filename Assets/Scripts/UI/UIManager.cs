using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        // 슬라이드 패널은 SetActive(false)가 아니라
        // 화면 밖으로 이동시켜 닫는다.
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

        // 일반 패널은 기존처럼 비활성화
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
}