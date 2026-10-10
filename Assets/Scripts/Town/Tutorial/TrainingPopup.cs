using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TrainingPopup : MonoBehaviour
{
    public static TrainingPopup Inst;

    [Header("Popup")]
    [SerializeField] GameObject popupPanel;

    [Header("Message")]
    [SerializeField] TMP_Text messageTMP;

    [TextArea]
    [SerializeField]
    string rewardMessage =
        "?";

    [TextArea]
    [SerializeField]
    string mockBattleMessage =
        "?";

    [Header("Confirm Buttons")]
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    bool canClose;
    bool isConfirmPopup;

    void Awake()
    {
        Inst = this;

        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(OnClickYes);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(OnClickNo);
        }
    }

    void Update()
    {
        if (isConfirmPopup || !canClose)
            return;

        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1))
        {
            Close();
        }
    }

    public void Show()
    {
        if (popupPanel == null)
            return;

        isConfirmPopup = false;
        canClose = false;

        if (messageTMP != null)
            messageTMP.text = rewardMessage;

        SetConfirmButtons(false);

        OpenPopup();

        StartCoroutine(EnableCloseNextFrame());
    }

    public void ShowMockBattleConfirm()
    {
        if (popupPanel == null)
            return;

        isConfirmPopup = true;
        canClose = false;

        if (messageTMP != null)
            messageTMP.text = mockBattleMessage;

        SetConfirmButtons(true);

        OpenPopup();
    }

    void OpenPopup()
    {
        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();
    }

    void SetConfirmButtons(bool visible)
    {
        if (yesButton != null)
            yesButton.gameObject.SetActive(visible);

        if (noButton != null)
            noButton.gameObject.SetActive(visible);
    }

    IEnumerator EnableCloseNextFrame()
    {
        yield return null;

        if (!isConfirmPopup)
            canClose = true;
    }

    void OnClickYes()
    {
        if (!isConfirmPopup)
            return;

        Close();

        if (TrainingSelectManager.Inst != null)
            TrainingSelectManager.Inst.StartMockBattle();
    }

    void OnClickNo()
    {
        if (!isConfirmPopup)
            return;

        Close();
    }

    public void Close()
    {
        canClose = false;
        isConfirmPopup = false;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}