using UnityEngine;
using DG.Tweening;

public class HandZoomCardHover : MonoBehaviour
{
    [SerializeField] float hoverY = 0.25f;
    [SerializeField] float hoverScale = 1.04f;
    [SerializeField] float duration = 0.12f;

    bool canHover;
    bool isHover;

    public bool CanHover => canHover;
    public bool IsHovering => isHover;
    public float HoverY => hoverY;
    public float HoverScale => hoverScale;

    Card card;
    Tween hoverMoveTween;
    Tween hoverScaleTween;

    void Awake()
    {
        card = GetComponent<Card>();
    }

    public void SetHoverActive(bool value)
    {
        canHover = value;

        if (!value)
            HoverOff();
    }

    public void HoverOn()
    {
        if (!canHover) return;
        if (isHover) return;
        if (card == null || !card.hasZoomPRS) return;

        isHover = true;

        hoverMoveTween?.Kill();
        hoverScaleTween?.Kill();

        Vector3 targetPos = card.zoomPRS.pos + new Vector3(0f, hoverY, 0f);
        Vector3 targetScale = card.zoomPRS.scale * hoverScale;

        hoverMoveTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.OutQuad);

        hoverScaleTween = transform.DOScale(targetScale, duration)
            .SetEase(Ease.OutQuad);
    }

    public void HoverOff()
    {
        if (!isHover) return;

        isHover = false;

        hoverMoveTween?.Kill();
        hoverScaleTween?.Kill();

        if (card == null || !card.hasZoomPRS)
            return;

        hoverMoveTween = transform.DOMove(card.zoomPRS.pos, duration)
            .SetEase(Ease.OutQuad);

        hoverScaleTween = transform.DOScale(card.zoomPRS.scale, duration)
            .SetEase(Ease.OutQuad);
    }

    void OnDisable()
    {
        hoverMoveTween?.Kill();
        hoverScaleTween?.Kill();

        canHover = false;
        isHover = false;
    }
}