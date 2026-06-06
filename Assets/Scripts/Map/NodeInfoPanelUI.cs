using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NodeInfoPanelUI : MonoBehaviour
{
    public static NodeInfoPanelUI Inst;

    [Header("Panel")]
    [SerializeField] RectTransform panelRect;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button closeButton;

    [Header("Reward Icons")]
    [SerializeField] GameObject[] rewardIconRoots;
    [SerializeField] Image[] rewardIcons;

    [Header("Slide")]
    [SerializeField] float hiddenX = 900f;
    [SerializeField] float shownX = 350f;
    [SerializeField] float duration = 0.35f;

    void Awake()
    {
        Inst = this;

        if (panelRect == null)
            panelRect = transform as RectTransform;

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() =>
            {
                if (NodeMapManager.Inst != null)
                    NodeMapManager.Inst.OnClickStartGame();
            });
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Hide);
        }

        HideInstant();
    }

    public void Show(NodeDataSO nodeData)
    {
        RefreshRewardIcons(nodeData);

        gameObject.SetActive(true);
        panelRect.DOKill();
        panelRect.DOAnchorPosX(shownX, duration).SetEase(Ease.OutCubic);
    }

    void RefreshRewardIcons(NodeDataSO nodeData)
    {
        HideRewardIcons();

        if (nodeData == null)
            return;

        if (nodeData.enemyDeck == null)
            return;

        if (nodeData.enemyDeck.rewardItems == null)
            return;

        int max = Mathf.Min(4, nodeData.enemyDeck.rewardItems.Count);

        for (int i = 0; i < max; i++)
        {
            DeckRewardItem reward = nodeData.enemyDeck.rewardItems[i];

            if (reward == null || reward.item == null || reward.count <= 0)
                continue;

            if (rewardIconRoots != null && i < rewardIconRoots.Length && rewardIconRoots[i] != null)
                rewardIconRoots[i].SetActive(true);

            if (rewardIcons != null && i < rewardIcons.Length && rewardIcons[i] != null)
                rewardIcons[i].sprite = reward.item.icon;
        }
    }

    void HideRewardIcons()
    {
        if (rewardIconRoots == null)
            return;

        for (int i = 0; i < rewardIconRoots.Length; i++)
        {
            if (rewardIconRoots[i] != null)
                rewardIconRoots[i].SetActive(false);
        }
    }

    public void Hide()
    {
        panelRect.DOKill();
        panelRect.DOAnchorPosX(hiddenX, duration).SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void HideInstant()
    {
        if (panelRect == null)
            panelRect = transform as RectTransform;

        panelRect.anchoredPosition = new Vector2(hiddenX, panelRect.anchoredPosition.y);
        gameObject.SetActive(false);
    }
}