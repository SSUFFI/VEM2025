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

        if (BattleTutorialManager.Inst != null && BattleTutorialManager.Inst.IsActive && BattleTutorialHighlight.Inst != null && panel != null)
        {
            BattleTutorialHighlight.Inst.Highlight(panel);
        }
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        if (BattleTutorialManager.Inst != null && BattleTutorialManager.Inst.IsActive)
        {
            BattleTutorialManager.Inst.OnCardPreviewClosed();
        }
    }

    public void OnBackgroundClick()
    {
        Hide();
    }
}