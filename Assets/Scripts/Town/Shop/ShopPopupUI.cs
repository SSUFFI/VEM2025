using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPopupUI : MonoBehaviour
{
    [Header("BackButton (Fixed Full Screen, Sibling of PopupUI)")]
    [SerializeField] Button backgroundCloseButton;

    [Header("Popup Root (Move This: Background + Contents)")]
    [SerializeField] RectTransform popupPanelRect; // = pnl_PopupUI
    [SerializeField] GameObject popupRoot;         // = pnl_PopupUI (same object ok)

    [Header("Canvas")]
    [SerializeField] Canvas rootCanvas;

    [Header("Texts")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text priceText;

    [Header("Buy Button")]
    [SerializeField] Button buyButton;

    ShopItemSO current;
    System.Action<ShopItemSO> onBuy;

    const float SCREEN_MARGIN = 18f;
    const float ABOVE_GAP = 12f;

    float ignoreBgUntil;

    void Awake()
    {
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();

        if (backgroundCloseButton != null)
        {
            backgroundCloseButton.onClick.RemoveAllListeners();
            backgroundCloseButton.onClick.AddListener(() =>
            {
                if (Time.unscaledTime < ignoreBgUntil) return;
                Hide();
            });
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                if (current != null) onBuy?.Invoke(current);
            });
        }

        Hide();
    }

    public bool IsOpen => popupRoot != null && popupRoot.activeSelf;

    public void Show(ShopItemSO item, RectTransform anchorRect, System.Action<ShopItemSO> buyCb)
    {
        current = item;
        onBuy = buyCb;

        if (nameText != null) nameText.text = item != null ? item.itemName : "";
        if (descText != null) descText.text = item != null ? item.description : "";
        if (priceText != null) priceText.text = item != null ? $"{item.price:N0}원" : "";

        if (popupRoot != null) popupRoot.SetActive(true);
        if (backgroundCloseButton != null) backgroundCloseButton.gameObject.SetActive(true);

        // 같은 클릭으로 바로 닫히는거 방지
        ignoreBgUntil = Time.unscaledTime + 0.12f;

        Canvas.ForceUpdateCanvases();
        PositionAboveAnchor(anchorRect);
    }

    void PositionAboveAnchor(RectTransform anchorRect)
    {
        if (popupPanelRect == null || rootCanvas == null || anchorRect == null) return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;

        Vector3[] a = new Vector3[4];
        anchorRect.GetWorldCorners(a);
        Vector3 anchorTopCenterWorld = (a[1] + a[2]) * 0.5f;

        Camera uiCam = (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(uiCam, anchorTopCenterWorld),
            uiCam,
            out localPoint
        );

        localPoint.y += ABOVE_GAP;
        popupPanelRect.pivot = new Vector2(0.5f, 0f);
        popupPanelRect.anchoredPosition = localPoint;

        ClampToCanvas(canvasRect, popupPanelRect, SCREEN_MARGIN, uiCam);
    }

    static void ClampToCanvas(RectTransform canvasRect, RectTransform panel, float margin, Camera uiCam)
    {
        Vector3[] p = new Vector3[4];
        panel.GetWorldCorners(p);

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        for (int i = 0; i < 4; i++)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                RectTransformUtility.WorldToScreenPoint(uiCam, p[i]),
                uiCam,
                out lp
            );
            min = Vector2.Min(min, lp);
            max = Vector2.Max(max, lp);
        }

        Rect c = canvasRect.rect;

        Vector2 delta = Vector2.zero;

        float leftLimit = c.xMin + margin;
        float rightLimit = c.xMax - margin;
        float bottomLimit = c.yMin + margin;
        float topLimit = c.yMax - margin;

        if (min.x < leftLimit) delta.x += (leftLimit - min.x);
        if (max.x > rightLimit) delta.x -= (max.x - rightLimit);
        if (min.y < bottomLimit) delta.y += (bottomLimit - min.y);
        if (max.y > topLimit) delta.y -= (max.y - topLimit);

        panel.anchoredPosition += delta;
    }

    public void Hide()
    {
        current = null;
        onBuy = null;

        if (backgroundCloseButton != null) backgroundCloseButton.gameObject.SetActive(false);
        if (popupRoot != null) popupRoot.SetActive(false);
    }
}