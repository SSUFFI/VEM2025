using UnityEngine;
using DG.Tweening;

public class TownSlidePanel : MonoBehaviour
{
    [SerializeField] RectTransform panel;

    [SerializeField] Vector2 openPos;
    [SerializeField] Vector2 closePos;

    bool opened;

    public bool IsOpen => opened;

    void Start()
    {
        panel.anchoredPosition = closePos;
        opened = false;
    }

    public void Toggle()
    {
        if (opened)
            Close();
        else
            Open();
    }

    public void Open()
    {
        opened = true;

        panel.DOKill();

        panel.DOAnchorPos(openPos, 0.35f)
            .SetEase(Ease.OutCubic);
    }

    public void Close()
    {
        opened = false;

        panel.DOKill();

        panel.DOAnchorPos(closePos, 0.3f)
            .SetEase(Ease.InCubic);
    }
}