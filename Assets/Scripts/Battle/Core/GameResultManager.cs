using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Inst;

    [Header("Panel")]
    public GameObject panel;

    [Header("Result UI")]
    public Image resultImage;
    public TMP_Text resultText;

    [Header("Buttons")]
    public Button continueButton;
    public Button exitButton;

    [Header("Sprites")]
    public Sprite winSprite;
    public Sprite loseSprite;

    [Header("Scene Names")]
    public string mapSceneName = "Map";
    public string townSceneName = "Town";


    public bool isGameOver = false;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
    }

    public void ShowWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        panel.SetActive(true);

        if (resultImage != null)
            resultImage.sprite = winSprite;

        if (resultText != null)
            resultText.text = "½Â¸®";

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnClickContinue);
        }

        if (exitButton != null)
            exitButton.gameObject.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ShowLose()
    {
        if (isGameOver) return;
        isGameOver = true;

        panel.SetActive(true);

        if (resultImage != null)
            resultImage.sprite = loseSprite;

        if (resultText != null)
            resultText.text = "ÆÐ¹è";

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnClickExit);
        }

        Time.timeScale = 0f;
    }

    void OnClickContinue()
    {
        Time.timeScale = 1f;

        int nodeID = PlayerPrefs.GetInt("SelectedNodeID", -1);

        PlayerPrefs.SetInt("ClearedNodeID", nodeID);
        PlayerPrefs.Save();

        SceneManager.LoadScene(mapSceneName);
    }

    void OnClickExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(townSceneName);
    }
}