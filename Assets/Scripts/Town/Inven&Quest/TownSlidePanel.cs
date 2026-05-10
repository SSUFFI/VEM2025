using UnityEngine;
using DG.Tweening;

public class TownSlidePanel : MonoBehaviour
{
    [SerializeField] RectTransform panel;

    [SerializeField] Vector2 openPos;
    [SerializeField] Vector2 closePos;

    bool opened;

    void Start()
    {
        panel.anchoredPosition = closePos;
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

        panel.DOAnchorPos(openPos, 0.35f)
            .SetEase(Ease.OutCubic);
    }

    public void Close()
    {
        opened = false;

        panel.DOAnchorPos(closePos, 0.3f)
            .SetEase(Ease.InCubic);
    }
}