using UnityEngine;

public class CardPreviewManager : MonoBehaviour
{
    public static CardPreviewManager Inst;

    [SerializeField] GameObject panel;
    [SerializeField] CardPreviewUI previewCard;

    public bool IsOpen => panel != null && panel.activeSelf;

    void Awake()
    {
        Inst = this;

        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(CardDataSO data)
    {
        if (data == null) return;

        if (panel != null)
            panel.SetActive(true);

        if (previewCard != null)
            previewCard.Setup(data);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void OnBackgroundClick()
    {
        Hide();
    }
}