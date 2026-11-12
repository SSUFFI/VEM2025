using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlateEventUI : MonoBehaviour
{
    [System.Serializable]
    public class SlateSlot
    {
        public Button button;
        public TMP_Text rewardNameText;
        public TMP_Text descriptionText;
        public Image rewardIcon;

        [Header("선택지 전체 밝기")]
        public CanvasGroup canvasGroup;

        [Header("필요 증표 이미지 (최대 3개)")]
        public Image[] requiredTokenImages;
    }

    [Header("7개 선택지 UI")]
    [SerializeField] private SlateSlot[] slots = new SlateSlot[7];

    [Header("증표 0개 - 고정 혜택")]
    [SerializeField] private SlateDataSO freeReward;

    [Header("증표 1개 - 무작위 2개")]
    [SerializeField] private List<SlateDataSO> oneTokenRewards;

    [Header("증표 2개 - 무작위 2개")]
    [SerializeField] private List<SlateDataSO> twoTokenRewards;

    [Header("증표 3개 - 무작위 2개")]
    [SerializeField] private List<SlateDataSO> threeTokenRewards;

    [Header("선택 완료 후 나가기")]
    [SerializeField] private GameObject exitButton;

    [Header("선택지 밝기")]
    [SerializeField, Range(0f, 1f)]
    private float unlockedAlpha = 1f;

    [SerializeField, Range(0f, 1f)]
    private float lockedAlpha = 0.35f;

    [Header("선택 애니메이션")]
    [SerializeField] private float selectedScale = 1.04f;
    [SerializeField] private float pulseAmount = 0.015f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float scaleSmoothSpeed = 12f;

    private readonly List<SlateDataSO> currentRewards =
        new List<SlateDataSO>();

    private SlateDataSO pendingReward;
    private int selectedSlotIndex = -1;
    private Coroutine selectionCoroutine;

    private void OnEnable()
    {
        InitializeSlate();
    }

    private void OnDisable()
    {
        StopSelectionAnimation();
        ResetSlotScales();
    }

    private void InitializeSlate()
    {
        StopSelectionAnimation();
        ResetSlotScales();

        pendingReward = null;
        selectedSlotIndex = -1;
        currentRewards.Clear();

        if (exitButton != null)
            exitButton.SetActive(false);

        if (freeReward == null ||
            oneTokenRewards == null ||
            twoTokenRewards == null ||
            threeTokenRewards == null ||
            slots == null ||
            slots.Length != 7)
        {
            Debug.LogError("SlateEventUI: 석판 설정이 올바르지 않습니다.");
            return;
        }

        currentRewards.Add(freeReward);

        if (!AddRandomRewards(oneTokenRewards, 2, 1) ||
            !AddRandomRewards(twoTokenRewards, 2, 2) ||
            !AddRandomRewards(threeTokenRewards, 2, 3))
        {
            Debug.LogError("SlateEventUI: 혜택 후보가 부족하거나 증표 조건이 잘못되었습니다.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
            SetupSlot(i, currentRewards[i]);
    }

    private bool AddRandomRewards(
        List<SlateDataSO> source,
        int count,
        int requiredCount)
    {
        List<SlateDataSO> candidates = new List<SlateDataSO>();

        foreach (SlateDataSO reward in source)
        {
            if (reward == null ||
                reward.requiredTokens == null ||
                reward.requiredTokens.Length != requiredCount ||
                candidates.Contains(reward))
                continue;

            candidates.Add(reward);
        }

        if (candidates.Count < count)
            return false;

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            currentRewards.Add(candidates[randomIndex]);
            candidates.RemoveAt(randomIndex);
        }

        return true;
    }

    private void SetupSlot(int index, SlateDataSO reward)
    {
        SlateSlot slot = slots[index];

        if (slot.button == null || reward == null)
            return;

        if (slot.rewardNameText != null)
            slot.rewardNameText.text = reward.rewardName;

        if (slot.descriptionText != null)
            slot.descriptionText.text = reward.description;

        if (slot.rewardIcon != null)
        {
            slot.rewardIcon.sprite = reward.rewardIcon;
            slot.rewardIcon.enabled = reward.rewardIcon != null;
        }

        if (slot.requiredTokenImages != null)
        {
            for (int i = 0; i < slot.requiredTokenImages.Length; i++)
            {
                Image image = slot.requiredTokenImages[i];

                if (image == null)
                    continue;

                if (i < reward.requiredTokens.Length &&
                    reward.requiredTokens[i] != null)
                {
                    image.sprite = reward.requiredTokens[i].tokenIcon;
                    image.enabled = image.sprite != null;
                }
                else
                {
                    image.sprite = null;
                    image.enabled = false;
                }
            }
        }

        bool unlocked = HasRequiredTokens(reward);

        slot.button.interactable = unlocked;

        if (slot.canvasGroup != null)
        {
            slot.canvasGroup.alpha =
                unlocked ? unlockedAlpha : lockedAlpha;
        }

        slot.button.onClick.RemoveAllListeners();

        int capturedIndex = index;
        slot.button.onClick.AddListener(
            () => SelectReward(capturedIndex));
    }

    private bool HasRequiredTokens(SlateDataSO reward)
    {
        if (reward == null || reward.requiredTokens == null)
            return false;

        if (reward.requiredTokens.Length == 0)
            return true;

        if (EventTokenManager.Instance == null)
            return false;

        foreach (TokenDataSO token in reward.requiredTokens)
        {
            if (token == null ||
                !EventTokenManager.Instance.HasToken(token))
                return false;
        }

        return true;
    }

    private void SelectReward(int index)
    {
        if (index < 0 || index >= currentRewards.Count)
            return;

        SlateDataSO reward = currentRewards[index];

        if (!HasRequiredTokens(reward))
            return;

        pendingReward = reward;
        selectedSlotIndex = index;

        if (exitButton != null)
            exitButton.SetActive(true);

        if (selectionCoroutine == null)
            selectionCoroutine = StartCoroutine(AnimateSelection());

        Debug.Log("석판 혜택 임시 선택: " + reward.rewardName);
    }

    private IEnumerator AnimateSelection()
    {
        while (true)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].button == null)
                    continue;

                Transform target = slots[i].button.transform;

                float targetScale = 1f;

                if (i == selectedSlotIndex)
                {
                    float pulse =
                        Mathf.Sin(Time.unscaledTime * pulseSpeed)
                        * pulseAmount;

                    targetScale = selectedScale + pulse;
                }

                Vector3 desiredScale = Vector3.one * targetScale;

                target.localScale = Vector3.Lerp(
                    target.localScale,
                    desiredScale,
                    Time.unscaledDeltaTime * scaleSmoothSpeed);
            }

            yield return null;
        }
    }

    private void StopSelectionAnimation()
    {
        if (selectionCoroutine == null)
            return;

        StopCoroutine(selectionCoroutine);
        selectionCoroutine = null;
    }

    private void ResetSlotScales()
    {
        if (slots == null)
            return;

        foreach (SlateSlot slot in slots)
        {
            if (slot != null && slot.button != null)
                slot.button.transform.localScale = Vector3.one;
        }
    }

    public void ConfirmSelection()
    {
        if (pendingReward == null)
            return;

        if (NodeMapManager.Inst == null)
            return;

        Debug.Log("석판 혜택 최종 확정: " +
                  pendingReward.rewardName);

        StopSelectionAnimation();
        ResetSlotScales();

        NodeMapManager.Inst.ClearSelectedNode();
        NodeMapManager.Inst.CloseAllEventPanels();
    }
}
