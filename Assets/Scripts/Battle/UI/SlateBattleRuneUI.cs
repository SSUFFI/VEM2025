using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlateBattleRuneUI : MonoBehaviour
{
    [Header("전투 화면의 고정 룬 버튼")]
    [SerializeField] private Button runeButton;
    [SerializeField] private Image runeImage;

    [Header("효과 설명 패널")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text effectText;

    [Header("설명 패널의 X 버튼")]
    [SerializeField] private Button closeButton;

    private SlateDataSO currentReward;
    private bool isPanelOpen;
    private bool isRuneVisible;

    private void OnEnable()
    {
        if (runeButton != null)
            runeButton.onClick.AddListener(ToggleDescription);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDescription);

        SlateRunState.Changed += Refresh;

        Refresh();
    }

    private void Start()
    {
        // 씬 내 다른 오브젝트의 Awake가 끝난 뒤 다시 확인합니다.
        Refresh();
    }

    private void OnDisable()
    {
        if (runeButton != null)
            runeButton.onClick.RemoveListener(ToggleDescription);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseDescription);

        SlateRunState.Changed -= Refresh;

        HideAll();
    }

    private void Update()
    {
        if (isRuneVisible && GameResultManager.IsBattleFinished)
            HideAll();
    }

    public void Refresh()
    {
        CloseDescription();

        currentReward = SlateRunState.Current;

        bool canShow =
            BattleData.battleMode == BattleMode.Normal &&
            !BattleData.isTutorialBattle &&
            BattleData.selectedNodeType == NodeType.Boss &&
            !GameResultManager.IsBattleFinished &&
            currentReward != null &&
            currentReward.showRune &&
            currentReward.RuneIcon != null;

        if (!canShow)
        {
            HideAll();
            return;
        }

        isRuneVisible = true;

        if (runeImage != null)
        {
            runeImage.sprite = currentReward.RuneIcon;
            runeImage.preserveAspect = true;
            runeImage.enabled = true;
        }

        if (runeButton != null)
        {
            runeButton.gameObject.SetActive(true);
            runeButton.interactable = true;
        }

        RefreshDescription();
    }

    private void RefreshDescription()
    {
        if (currentReward == null)
            return;

        if (titleText != null)
            titleText.text = currentReward.rewardName;

        if (effectText != null)
        {
            effectText.text = currentReward.EffectDescription;

            if (currentReward.effectType == SlateEffectType.GrantGold)
            {
                effectText.text +=
                    "\n\n석판 선택 시 금화 지급이 완료되었습니다.";
            }
        }
    }

    public void ToggleDescription()
    {
        if (!isRuneVisible ||
            currentReward == null ||
            descriptionPanel == null)
        {
            return;
        }

        if (GameResultManager.IsBattleFinished)
        {
            HideAll();
            return;
        }

        if (isPanelOpen)
        {
            CloseDescription();
            return;
        }

        RefreshDescription();

        isPanelOpen = true;
        descriptionPanel.SetActive(true);

        // 같은 부모 아래의 다른 UI보다 앞에 표시합니다.
        descriptionPanel.transform.SetAsLastSibling();
    }

    public void CloseDescription()
    {
        isPanelOpen = false;

        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    private void HideAll()
    {
        CloseDescription();

        isRuneVisible = false;
        currentReward = null;

        if (runeButton != null)
            runeButton.gameObject.SetActive(false);

        if (runeImage != null)
        {
            runeImage.sprite = null;
            runeImage.enabled = false;
        }
    }
}