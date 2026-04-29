using UnityEngine;
using UnityEngine.EventSystems;

public class GravePreviewClick : MonoBehaviour, IPointerClickHandler
{
    GraveUICard graveCard;

    void Awake()
    {
        graveCard = GetComponentInParent<GraveUICard>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (graveCard == null)
            return;

        graveCard.ShowPreview();
    }
}