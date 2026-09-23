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

    [Header("Reward Gold")]
    [SerializeField] GameObject goldRoot;
    [SerializeField] Image goldIcon;
    [SerializeField] TMP_Text goldText;

    [Header("Reward Items")]
    [SerializeField] GameObject[] itemRoots;
    [SerializeField] Image[] itemIcons;
    [SerializeField] TMP_Text[] itemTexts;

    public bool isGameOver = false;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
        HideRewards();
    }

    public void ShowWin()
    {
        if (isGameOver)
            return;

        // 새 전투 튜토리얼 1~3
        if (BattleData.IsBattleTutorial &&
            BattleTutorialManager.Inst != null)
        {
            BattleTutorialManager.Inst
                .OnTutorialWin();

            return;
        }

        isGameOver = true;

        panel.SetActive(true);

        if (resultImage != null)
            resultImage.sprite = winSprite;

        if (resultText != null)
            resultText.text = "승리";

        // 훈련장의 모의전투
        if (BattleData.isTutorialBattle)
        {
            HideRewards();

            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(
                    OnClickMockBattleReturn);
            }

            if (exitButton != null)
                exitButton.gameObject.SetActive(false);

            return;
        }

        GiveAndShowRewards();

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(
                OnClickContinue);
        }

        if (exitButton != null)
            exitButton.gameObject.SetActive(false);
    }

    public void ShowLose()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        panel.SetActive(true);
        HideRewards();

        if (resultImage != null)
            resultImage.sprite = loseSprite;

        if (resultText != null)
            resultText.text = "패배";

        // 모의전투는 패배해도 훈련장으로 복귀.
        if (BattleData.isTutorialBattle)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(false);

            if (exitButton != null)
            {
                exitButton.gameObject.SetActive(true);
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(
                    OnClickMockBattleReturn);
            }

            return;
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(
                OnClickExit);
        }
    }

    void GiveAndShowRewards()
    {
        HideRewards();

        DeckSO deck =
            BattleData.selectedEnemyDeck;

        if (deck == null)
            return;

        if (deck.goldItem != null &&
            deck.goldAmount > 0)
        {
            if (InventoryManager.Inst != null)
            {
                InventoryManager.Inst.AddItem(
                    deck.goldItem,
                    deck.goldAmount);
            }

            if (goldRoot != null)
                goldRoot.SetActive(true);

            if (goldIcon != null)
                goldIcon.sprite =
                    deck.goldItem.icon;

            if (goldText != null)
            {
                goldText.text =
                    $"{deck.goldItem.itemName} x{deck.goldAmount}";
            }
        }

        int max =
            Mathf.Min(
                4,
                deck.rewardItems.Count);

        for (int i = 0; i < max; i++)
        {
            DeckRewardItem reward =
                deck.rewardItems[i];

            if (reward == null ||
                reward.item == null ||
                reward.count <= 0)
            {
                continue;
            }

            if (InventoryManager.Inst != null)
            {
                InventoryManager.Inst.AddItem(
                    reward.item,
                    reward.count);
            }

            if (itemRoots != null &&
                i < itemRoots.Length &&
                itemRoots[i] != null)
            {
                itemRoots[i].SetActive(true);
            }

            if (itemIcons != null &&
                i < itemIcons.Length &&
                itemIcons[i] != null)
            {
                itemIcons[i].sprite =
                    reward.item.icon;
            }

            if (itemTexts != null &&
                i < itemTexts.Length &&
                itemTexts[i] != null)
            {
                itemTexts[i].text =
                    $"{reward.item.itemName} x{reward.count}";
            }
        }
    }

    void HideRewards()
    {
        if (goldRoot != null)
            goldRoot.SetActive(false);

        if (itemRoots != null)
        {
            for (int i = 0;
                 i < itemRoots.Length;
                 i++)
            {
                if (itemRoots[i] != null)
                    itemRoots[i].SetActive(false);
            }
        }
    }

    void OnClickMockBattleReturn()
    {
        Time.timeScale = 1f;

        TrainingSelectManager
            .OpenPanelAfterReturn();

        BattleData.isTutorialBattle = false;
        BattleData.tutorialEnemyDeck = null;
        BattleData.selectedEnemyDeck = null;

        SceneManager.LoadScene(
            townSceneName);
    }

    void OnClickContinue()
    {
        Time.timeScale = 1f;

        if (BattleData.selectedNodeType ==
            NodeType.Boss)
        {
            StageProgress.highestClearedStage =
                Mathf.Max(
                    StageProgress.highestClearedStage,
                    StageProgress.selectedStage);

            NodeMapRuntimeData.ResetRun();

            SceneManager.LoadScene(
                townSceneName);

            return;
        }

        int nodeID =
            PlayerPrefs.GetInt(
                "SelectedNodeID",
                -1);

        PlayerPrefs.SetInt(
            "ClearedNodeID",
            nodeID);

        PlayerPrefs.Save();

        SceneManager.LoadScene(
            mapSceneName);
    }

    void OnClickExit()
    {
        Time.timeScale = 1f;

        BattleData.isTutorialBattle = false;
        BattleData.tutorialEnemyDeck = null;

        NodeMapRuntimeData.ResetRun();

        SceneManager.LoadScene(
            townSceneName);
    }
}