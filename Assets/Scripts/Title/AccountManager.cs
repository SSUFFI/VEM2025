using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Inst { get; private set; }

    [Header("Account Panel")]
    [SerializeField] GameObject accountSlotPanel;

    [Header("Account Buttons")]
    [SerializeField] Button accountAButton;
    [SerializeField] Button accountBButton;
    [SerializeField] Button accountCButton;

    [Header("Delete Buttons")]
    [SerializeField] Button deleteAButton;
    [SerializeField] Button deleteBButton;
    [SerializeField] Button deleteCButton;

    [Header("Play Time Text")]
    [SerializeField] TMP_Text accountATimeText;
    [SerializeField] TMP_Text accountBTimeText;
    [SerializeField] TMP_Text accountCTimeText;

    [Header("Current Account UI")]
    [SerializeField] TMP_Text currentAccountText;

    [Header("Buttons")]
    [SerializeField] Button gameStartButton;
    [SerializeField] Button loginAnotherAccountButton;

    const string CURRENT_ACCOUNT_KEY = "CurrentAccount";

    string currentAccount = "";

    float playTimeSaveTimer;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SetupButtons();
        LoadCurrentAccount();
        RefreshUI();
    }

    void Update()
    {
        if (string.IsNullOrEmpty(currentAccount))
            return;

        float currentPlayTime =
            PlayerPrefs.GetFloat(
                GetPlayTimeKey(currentAccount),
                0f);

        currentPlayTime += Time.unscaledDeltaTime;

        PlayerPrefs.SetFloat(
            GetPlayTimeKey(currentAccount),
            currentPlayTime);

        playTimeSaveTimer += Time.unscaledDeltaTime;

        if (playTimeSaveTimer >= 10f)
        {
            playTimeSaveTimer = 0f;
            PlayerPrefs.Save();
        }

        RefreshPlayTimeUI();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    void SetupButtons()
    {
        if (accountAButton != null)
        {
            accountAButton.onClick.RemoveAllListeners();
            accountAButton.onClick.AddListener(
                () => SelectAccount("A"));
        }

        if (accountBButton != null)
        {
            accountBButton.onClick.RemoveAllListeners();
            accountBButton.onClick.AddListener(
                () => SelectAccount("B"));
        }

        if (accountCButton != null)
        {
            accountCButton.onClick.RemoveAllListeners();
            accountCButton.onClick.AddListener(
                () => SelectAccount("C"));
        }

        if (deleteAButton != null)
        {
            deleteAButton.onClick.RemoveAllListeners();
            deleteAButton.onClick.AddListener(
                () => DeleteAccount("A"));
        }

        if (deleteBButton != null)
        {
            deleteBButton.onClick.RemoveAllListeners();
            deleteBButton.onClick.AddListener(
                () => DeleteAccount("B"));
        }

        if (deleteCButton != null)
        {
            deleteCButton.onClick.RemoveAllListeners();
            deleteCButton.onClick.AddListener(
                () => DeleteAccount("C"));
        }

        if (loginAnotherAccountButton != null)
        {
            loginAnotherAccountButton.onClick.RemoveAllListeners();
            loginAnotherAccountButton.onClick.AddListener(
                OpenAccountPanel);
        }

    }

    void LoadCurrentAccount()
    {
        currentAccount =
            PlayerPrefs.GetString(
                CURRENT_ACCOUNT_KEY,
                "");
    }

    void SelectAccount(string account)
    {
        currentAccount = account;

        PlayerPrefs.SetString(
            CURRENT_ACCOUNT_KEY,
            currentAccount);

        PlayerPrefs.SetInt(
            GetExistsKey(currentAccount),
            1);

        PlayerPrefs.Save();

        if (accountSlotPanel != null)
            accountSlotPanel.SetActive(false);

        RefreshUI();

        Debug.Log(
            $"프로필 {currentAccount} 선택");
    }

    void DeleteAccount(string account)
    {
        DeleteAccountData(account);

        if (currentAccount == account)
        {
            currentAccount = "";

            PlayerPrefs.DeleteKey(
                CURRENT_ACCOUNT_KEY);
        }

        PlayerPrefs.Save();

        RefreshUI();

        Debug.Log(
            $"프로필 {account} 삭제");
    }

    void DeleteAccountData(string account)
    {
        PlayerPrefs.DeleteKey(
            GetExistsKey(account));

        PlayerPrefs.DeleteKey(
            GetPlayTimeKey(account));

        PlayerPrefs.DeleteKey(
            GetAccountKey(
                account,
                "TutorialDone"));

        PlayerPrefs.DeleteKey(
             GetAccountKey(
                 account,
                 "Inventory"));

        PlayerPrefs.DeleteKey(
             GetAccountKey(
                account,
                "CardPool"));

        PlayerPrefs.DeleteKey(
            GetAccountKey(
                account,
                "SavedDeck"));

        PlayerPrefs.DeleteKey(
            GetAccountKey(
                account,
                "HighestClearedStage"));

        PlayerPrefs.DeleteKey(
            GetAccountKey(
                account,
                "Shop_LastRefresh"));

        for (int i = 0; i < 12; i++)
        {
            PlayerPrefs.DeleteKey(
                GetAccountKey(
                    account,
                    $"Shop_Item_{i}"));

            PlayerPrefs.DeleteKey(
                GetAccountKey(
                    account,
                    $"Shop_SoldOut_{i}"));
        }
    }

    public void OpenAccountPanel()
    {
        if (accountSlotPanel != null)
            accountSlotPanel.SetActive(true);

        RefreshUI();
    }

    public void CloseAccountPanel()
    {
        if (accountSlotPanel != null)
            accountSlotPanel.SetActive(false);
    }

    void RefreshUI()
    {
        RefreshCurrentAccountUI();
        RefreshPlayTimeUI();
    }

    void RefreshCurrentAccountUI()
    {
        if (currentAccountText != null)
        {
            if (string.IsNullOrEmpty(currentAccount))
            {
                currentAccountText.text =
                    "현재 프로필 : ?";
            }
            else
            {
                currentAccountText.text =
                    $"현재 프로필 : {currentAccount}";
            }
        }

        if (gameStartButton != null)
        {
            gameStartButton.interactable =
                !string.IsNullOrEmpty(
                    currentAccount);
        }
    }

    void RefreshPlayTimeUI()
    {
        if (accountATimeText != null)
        {
            accountATimeText.text =
                FormatPlayTime(
                    GetPlayTime("A"));
        }

        if (accountBTimeText != null)
        {
            accountBTimeText.text =
                FormatPlayTime(
                    GetPlayTime("B"));
        }

        if (accountCTimeText != null)
        {
            accountCTimeText.text =
                FormatPlayTime(
                    GetPlayTime("C"));
        }
    }

    float GetPlayTime(string account)
    {
        return PlayerPrefs.GetFloat(
            GetPlayTimeKey(account),
            0f);
    }

    string FormatPlayTime(float seconds)
    {
        int totalSeconds =
            Mathf.FloorToInt(seconds);

        int hours =
            totalSeconds / 3600;

        int minutes =
            (totalSeconds % 3600) / 60;

        int secs =
            totalSeconds % 60;

        return
            $"{hours:00}:{minutes:00}:{secs:00}";
    }

    public string GetCurrentAccount()
    {
        return currentAccount;
    }

    public bool HasCurrentAccount()
    {
        return !string.IsNullOrEmpty(
            currentAccount);
    }

    public static string GetAccountKey(
    string baseKey)
    {
        string account =
            PlayerPrefs.GetString(
                CURRENT_ACCOUNT_KEY,
                "");

        if (string.IsNullOrEmpty(account))
            return baseKey;

        return
            $"Account_{account}_{baseKey}";
    }

    public static string GetAccountKey(
        string account,
        string baseKey)
    {
        return
            $"Account_{account}_{baseKey}";
    }

    static string GetExistsKey(
        string account)
    {
        return
            $"Account_{account}_Exists";
    }

    static string GetPlayTimeKey(
        string account)
    {
        return
            $"Account_{account}_PlayTime";
    }
}