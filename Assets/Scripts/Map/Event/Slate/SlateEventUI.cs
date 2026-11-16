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
    [SerializeField]
    private SlateSlot[] slots = new SlateSlot[7];

    [Header("증표 0개 - 고정 혜택")]
    [SerializeField]
    private SlateDataSO freeReward;

    [Header("증표 1개 - 보유 증표 연동 2개")]
    [SerializeField]
    private List<SlateDataSO> oneTokenRewards;

    [Header("증표 2개 - 보유 증표 연동 2개")]
    [SerializeField]
    private List<SlateDataSO> twoTokenRewards;

    [Header("증표 3개 - 보유 증표 연동 2개")]
    [SerializeField]
    private List<SlateDataSO> threeTokenRewards;

    [Header("선택 완료 후 나가기")]
    [SerializeField]
    private GameObject exitButton;

    [Header("선택지 밝기")]
    [SerializeField, Range(0f, 1f)]
    private float unlockedAlpha = 1f;

    [SerializeField, Range(0f, 1f)]
    private float lockedAlpha = 0.35f;

    [Header("선택 애니메이션")]
    [SerializeField]
    private float selectedScale = 1.04f;

    [SerializeField]
    private float pulseAmount = 0.015f;

    [SerializeField]
    private float pulseSpeed = 3f;

    [SerializeField]
    private float scaleSmoothSpeed = 12f;

    private readonly List<SlateDataSO> currentRewards =
        new List<SlateDataSO>();

    // 이번 석판 화면에서만 사용하는 증표 조건입니다.
    // 원본 SlateDataSO.requiredTokens는 수정하지 않습니다.
    private readonly Dictionary<SlateDataSO, TokenDataSO[]>
        runtimeRequirements =
            new Dictionary<SlateDataSO, TokenDataSO[]>();

    private bool selectionConfirmed;
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

        selectionConfirmed = false;
        pendingReward = null;
        selectedSlotIndex = -1;

        currentRewards.Clear();
        runtimeRequirements.Clear();

        if (exitButton != null)
            exitButton.SetActive(false);

        if (freeReward == null ||
            oneTokenRewards == null ||
            twoTokenRewards == null ||
            threeTokenRewards == null ||
            slots == null ||
            slots.Length != 7)
        {
            Debug.LogError(
                "SlateEventUI: 석판 설정이 올바르지 않습니다.");

            return;
        }

        currentRewards.Add(freeReward);

        if (!AddRandomRewards(oneTokenRewards, 2, 1) ||
            !AddRandomRewards(twoTokenRewards, 2, 2) ||
            !AddRandomRewards(threeTokenRewards, 2, 3))
        {
            Debug.LogError(
                "SlateEventUI: 각 단계에 유효한 석판이 " +
                "최소 2개 필요합니다. 증표 조건도 확인하세요.");

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
        if (source == null)
            return false;

        List<SlateDataSO> candidates =
            new List<SlateDataSO>();

        foreach (SlateDataSO reward in source)
        {
            if (reward == null ||
                reward.requiredTokens == null ||
                reward.requiredTokens.Length != requiredCount ||
                candidates.Contains(reward))
            {
                continue;
            }

            // 빈 참조 또는 중복 증표 조건이 있는 데이터는 제외합니다.
            HashSet<TokenDataSO> unique =
                new HashSet<TokenDataSO>();

            bool valid = true;

            foreach (TokenDataSO token in reward.requiredTokens)
            {
                if (token == null || !unique.Add(token))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                candidates.Add(reward);
        }

        if (candidates.Count < count)
            return false;

        List<TokenDataSO> owned = GetOwnedTokens();

        for (int i = 0; i < count; i++)
        {
            int bestScore = -1;
            int bestIndex = -1;
            int tieCount = 0;

            // 보유 증표와 많이 일치하는 후보부터 선택합니다.
            // 일치 개수가 같으면 해당 후보들 사이에서 무작위입니다.
            for (int j = 0; j < candidates.Count; j++)
            {
                int score = CountOwnedRequirements(
                    candidates[j],
                    owned);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = j;
                    tieCount = 1;
                }
                else if (score == bestScore)
                {
                    tieCount++;

                    if (Random.Range(0, tieCount) == 0)
                        bestIndex = j;
                }
            }

            SlateDataSO selected = candidates[bestIndex];

            // 같은 석판이 중복으로 나오지 않도록 제거합니다.
            candidates.RemoveAt(bestIndex);

            TokenDataSO[] requirements =
                selected.requiredTokens;

            // 필요한 개수를 가지고 있지만 원래 조합과 다르다면,
            // 실제 보유 증표로 이번 선택지의 조건을 구성합니다.
            if (owned.Count >= requiredCount &&
                bestScore < requiredCount)
            {
                requirements = BuildOwnedRequirements(
                    selected,
                    owned);
            }

            runtimeRequirements[selected] = requirements;
            currentRewards.Add(selected);
        }

        return true;
    }

    private List<TokenDataSO> GetOwnedTokens()
    {
        List<TokenDataSO> result =
            new List<TokenDataSO>();

        if (EventTokenManager.Instance == null)
            return result;

        foreach (TokenDataSO token in
                 EventTokenManager.Instance.OwnedTokens)
        {
            if (token != null && !result.Contains(token))
                result.Add(token);
        }

        return result;
    }

    private int CountOwnedRequirements(
        SlateDataSO reward,
        List<TokenDataSO> owned)
    {
        int count = 0;

        foreach (TokenDataSO token in reward.requiredTokens)
        {
            if (owned.Contains(token))
                count++;
        }

        return count;
    }

    private TokenDataSO[] BuildOwnedRequirements(
        SlateDataSO reward,
        List<TokenDataSO> owned)
    {
        List<TokenDataSO> result =
            new List<TokenDataSO>();

        List<TokenDataSO> remaining =
            new List<TokenDataSO>(owned);

        // 원래 요구 증표 중 가지고 있는 증표는 유지합니다.
        foreach (TokenDataSO token in reward.requiredTokens)
        {
            if (remaining.Remove(token))
                result.Add(token);
        }

        // 대체할 증표의 선택 순서를 무작위로 섞습니다.
        for (int i = remaining.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            TokenDataSO temp = remaining[i];
            remaining[i] = remaining[j];
            remaining[j] = temp;
        }

        foreach (TokenDataSO original in reward.requiredTokens)
        {
            if (result.Contains(original))
                continue;

            // 가능하면 같은 출처의 증표로 대체합니다.
            // 예: 탐색 증표 → 현재 보유한 다른 탐색 증표.
            int index = remaining.FindIndex(
                token => token.source == original.source);

            // 같은 출처가 없으면 남아 있는 보유 증표를 사용합니다.
            if (index < 0)
                index = 0;

            result.Add(remaining[index]);
            remaining.RemoveAt(index);
        }

        return result.ToArray();
    }

    private TokenDataSO[] GetRequiredTokens(SlateDataSO reward)
    {
        if (reward == null)
            return null;

        if (runtimeRequirements.TryGetValue(
                reward,
                out TokenDataSO[] requirements))
        {
            return requirements;
        }

        return reward.requiredTokens;
    }

    private void SetupSlot(int index, SlateDataSO reward)
    {
        SlateSlot slot = slots[index];

        TokenDataSO[] requirements =
            GetRequiredTokens(reward);

        if (slot == null ||
            slot.button == null ||
            reward == null ||
            requirements == null)
        {
            return;
        }

        if (slot.rewardNameText != null)
            slot.rewardNameText.text = reward.rewardName;

        if (slot.descriptionText != null)
            slot.descriptionText.text = reward.description;

        if (slot.rewardIcon != null)
        {
            slot.rewardIcon.sprite = reward.rewardIcon;
            slot.rewardIcon.enabled = reward.rewardIcon != null;
        }

        // 원본 조건이 아닌, 이번에 실제로 사용하는 조건을 표시합니다.
        if (slot.requiredTokenImages != null)
        {
            for (int i = 0; i < slot.requiredTokenImages.Length; i++)
            {
                Image image = slot.requiredTokenImages[i];

                if (image == null)
                    continue;

                if (i < requirements.Length &&
                    requirements[i] != null)
                {
                    image.sprite = requirements[i].tokenIcon;
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
        TokenDataSO[] requirements =
            GetRequiredTokens(reward);

        if (requirements == null)
            return false;

        if (requirements.Length == 0)
            return true;

        if (EventTokenManager.Instance == null)
            return false;

        foreach (TokenDataSO token in requirements)
        {
            if (token == null ||
                !EventTokenManager.Instance.HasToken(token))
            {
                return false;
            }
        }

        return true;
    }

    private void SelectReward(int index)
    {
        if (selectionConfirmed ||
            index < 0 ||
            index >= currentRewards.Count)
        {
            return;
        }

        SlateDataSO reward = currentRewards[index];

        if (!HasRequiredTokens(reward))
            return;

        pendingReward = reward;
        selectedSlotIndex = index;

        if (exitButton != null)
            exitButton.SetActive(true);

        if (selectionCoroutine == null)
            selectionCoroutine = StartCoroutine(AnimateSelection());

        Debug.Log(
            "석판 혜택 임시 선택: " +
            reward.rewardName);
    }

    private IEnumerator AnimateSelection()
    {
        while (true)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null ||
                    slots[i].button == null)
                {
                    continue;
                }

                Transform target = slots[i].button.transform;

                float targetScale = 1f;

                if (i == selectedSlotIndex)
                {
                    float pulse =
                        Mathf.Sin(Time.unscaledTime * pulseSpeed) *
                        pulseAmount;

                    targetScale = selectedScale + pulse;
                }

                Vector3 desiredScale =
                    Vector3.one * targetScale;

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
        if (selectionConfirmed ||
            pendingReward == null ||
            !HasRequiredTokens(pendingReward))
        {
            return;
        }

        if (NodeMapManager.Inst == null)
            return;

        if (FadeManager.Instance != null &&
            FadeManager.Instance.IsTransitioning)
        {
            return;
        }

        // 앞에서 만든 룬 저장 및 보스전 적용 연결을 유지합니다.
        if (!SlateRunState.TrySelect(pendingReward))
            return;

        selectionConfirmed = true;

        if (exitButton != null)
            exitButton.SetActive(false);

        foreach (SlateSlot slot in slots)
        {
            if (slot != null && slot.button != null)
                slot.button.interactable = false;
        }

        Debug.Log(
            "석판 혜택 최종 확정: " +
            pendingReward.rewardName);

        StopSelectionAnimation();
        ResetSlotScales();

        NodeMapManager.Inst.ClearSelectedNode();
        NodeMapManager.Inst.CloseAllEventPanels();
    }
}