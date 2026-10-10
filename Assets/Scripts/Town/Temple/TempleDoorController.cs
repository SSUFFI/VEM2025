using UnityEngine;
using DG.Tweening;

public class TempleDoorController : MonoBehaviour
{
    public static TempleDoorController Inst;

    [Header("Doors")]
    [SerializeField] RectTransform leftDoor;
    [SerializeField] RectTransform rightDoor;

    [Header("Slide Settings")]
    [SerializeField] float moveDistance = 250f;
    [SerializeField] float duration = 1f;

    Vector2 leftStartPos;
    Vector2 rightStartPos;

    void Awake()
    {
        Inst = this;

        leftStartPos = leftDoor.anchoredPosition;
        rightStartPos = rightDoor.anchoredPosition;
    }

    public void OpenDoors()
    {
        if (leftDoor == null || rightDoor == null)
            return;

        leftDoor.DOKill();
        rightDoor.DOKill();

        leftDoor.anchoredPosition = leftStartPos;
        rightDoor.anchoredPosition = rightStartPos;

        leftDoor.gameObject.SetActive(true);
        rightDoor.gameObject.SetActive(true);

        leftDoor.DOAnchorPosX(leftStartPos.x - moveDistance, duration).SetEase(Ease.InOutCubic);

        rightDoor.DOAnchorPosX(rightStartPos.x + moveDistance, duration).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            leftDoor.gameObject.SetActive(false);
            rightDoor.gameObject.SetActive(false);
        });
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}