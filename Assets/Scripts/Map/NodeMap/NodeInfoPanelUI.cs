using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    //JS 임시
    [Header("Battle Node Information")]
    [SerializeField] Image battleInfoImage;

    [Header("Event Information")]
    [SerializeField] Image eventInfoImage;

    [Header("Common Text")]
    [SerializeField] TMP_Text infoTitleText;
    [SerializeField] TMP_Text infoDescriptionText;

    [Header("Reward / Event Label")]
    [SerializeField] TMP_Text rewardLabelText;
    //===

    [Header("Slide")]
    [SerializeField] float hiddenX = 900f;
    [SerializeField] float shownX = 350f;
    [SerializeField] float duration = 0.35f;

    [Header("Close Area")]
    [SerializeField] GameObject nodeInfoCloseArea;

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

        if (nodeInfoCloseArea != null)
        {
            Button closeAreaButton = nodeInfoCloseArea.GetComponent<Button>();

            if (closeAreaButton != null)
            {
                closeAreaButton.onClick.RemoveAllListeners();
                closeAreaButton.onClick.AddListener(Hide);
            }
        }

        HideInstant();
    }

    public void Show(NodeDataSO nodeData)
    {
        RefreshNodeInfo(nodeData);

        if (nodeInfoCloseArea != null)
            nodeInfoCloseArea.SetActive(true);

        gameObject.SetActive(true);

        panelRect.DOKill();
        panelRect.DOAnchorPosX(shownX, duration).SetEase(Ease.OutCubic);
    }

    //JS임시
    void RefreshNodeInfo(NodeDataSO nodeData)
    {
        HideRewardIcons();
        SetInfo(null, "", "");

        if (rewardLabelText != null)
            rewardLabelText.text = "";

        if (nodeData == null)
            return;

        // 이벤트 노드
        if (nodeData.nodeType == NodeType.Event)
        {
            string title =
                string.IsNullOrWhiteSpace(nodeData.eventTitle)
                    ? GetEventName(nodeData.eventType)
                    : nodeData.eventTitle;

            Sprite image = nodeData.eventPreviewImage != null
                ? nodeData.eventPreviewImage
                : nodeData.iconNormal;

            SetInfo(
                image,
                title,
                nodeData.eventDescription,

                true
            );

            if (rewardLabelText != null)
                rewardLabelText.text = "이벤트";

            return;
        }

        if (nodeData.nodeType != NodeType.Battle &&
            nodeData.nodeType != NodeType.Boss)
        {
            return;
        }

        // 전투 / 보스 노드
        DeckSO deck = nodeData.enemyDeck;

        string defaultTitle = nodeData.nodeType == NodeType.Boss
            ? "보스 전투"
            : "전투";

        if (deck != null)
        {
            string title =
                string.IsNullOrWhiteSpace(deck.enemyName)
                    ? defaultTitle
                    : deck.enemyName;

            SetInfo(
                deck.heroPortrait,
                title,
                deck.enemyDescription
            );

            RefreshRewardIcons(nodeData);
        }
        else
        {
            SetInfo(null, defaultTitle, "");
        }

        if (rewardLabelText != null)
            rewardLabelText.text = "보상 아이템";
    }

    void SetInfo(
     Sprite image,
     string title,
     string description,
     bool isEvent = false)
    {
        // 이전에 표시한 이미지를 모두 숨깁니다.
        if (battleInfoImage != null)
        {
            battleInfoImage.sprite = null;
            battleInfoImage.gameObject.SetActive(false);
        }

        if (eventInfoImage != null)
        {
            eventInfoImage.sprite = null;
            eventInfoImage.gameObject.SetActive(false);
        }

        Image selectedImage = isEvent
            ? eventInfoImage
            : battleInfoImage;

        if (selectedImage != null && image != null)
        {
            selectedImage.sprite = image;
            selectedImage.enabled = true;
            selectedImage.preserveAspect = true;
            selectedImage.gameObject.SetActive(true);
        }

        if (infoTitleText != null)
            infoTitleText.text = title ?? "";

        if (infoDescriptionText != null)
            infoDescriptionText.text = description ?? "";
    }

    string GetEventName(EventType type)
    {
        switch (type)
        {
            case EventType.NPC:
                return "NPC 이벤트";

            case EventType.Heal:
                return "회복 이벤트";

            case EventType.Explore:
                return "탐색 이벤트";

            case EventType.Tablet:
                return "석판 이벤트";

            default:
                return "이벤트";
        }
    }
    //===
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

            //JS수정
            if (rewardIcons != null &&  i < rewardIcons.Length && rewardIcons[i] != null)
            {
                rewardIcons[i].sprite = reward.item.icon;
                rewardIcons[i].enabled = reward.item.icon != null;
            }
            //===
        }
    }

    //JS수정
    void HideRewardIcons()
    {
        if (rewardIcons != null)
        {
            foreach (Image icon in rewardIcons)
            {
                if (icon == null)
                    continue;

                icon.sprite = null;
                icon.enabled = false;
            }
        }

        if (rewardIconRoots != null)
        {
            foreach (GameObject root in rewardIconRoots)
            {
                if (root != null)
                    root.SetActive(false);
            }
        }
    }
    //===
    public void Hide()
    {
        if (nodeInfoCloseArea != null)
            nodeInfoCloseArea.SetActive(false);

        panelRect.DOKill();
        panelRect.DOAnchorPosX(hiddenX, duration).SetEase(Ease.InCubic).OnComplete(() => gameObject.SetActive(false));
    }

    public void HideInstant()
    {
        if (panelRect == null)
            panelRect = transform as RectTransform;

        panelRect.anchoredPosition = new Vector2(hiddenX, panelRect.anchoredPosition.y);

        if (nodeInfoCloseArea != null)
            nodeInfoCloseArea.SetActive(false);

        gameObject.SetActive(false);
    }
}