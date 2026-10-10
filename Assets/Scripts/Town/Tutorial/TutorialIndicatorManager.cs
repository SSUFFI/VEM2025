
using UnityEngine;

public class TutorialIndicatorManager : MonoBehaviour
{
    public static TutorialIndicatorManager Inst;

    [Header("Indicator")]
    [SerializeField] RectTransform indicator;

    [Header("Animation")]
    [SerializeField] float floatHeight = 10f;
    [SerializeField] float floatSpeed = 3f;

    RectTransform currentTarget;
    Vector2 currentOffset;
    float currentScale = 1f;
    float animationTime;

    void Awake()
    {
        Inst = this;

        if (indicator != null)
            indicator.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (currentTarget == null || indicator == null)
            return;

        if (!indicator.gameObject.activeSelf)
            return;

        UpdateIndicatorPosition();
    }

    public void Show(RectTransform target, float offsetY = 30f, float scale = 1f)
    {
        if (target == null || indicator == null)
            return;

        currentTarget = target;
        currentOffset = new Vector2(0f, offsetY);
        currentScale = scale;
        animationTime = 0f;

        indicator.localScale =
            Vector3.one * currentScale;

        indicator.gameObject.SetActive(true);

        UpdateIndicatorPosition();
    }

    public void Hide()
    {
        currentTarget = null;

        if (indicator != null)
            indicator.gameObject.SetActive(false);
    }

    public void Suspend()
    {

        if (indicator != null)
            indicator.gameObject.SetActive(false);
    }

    public void Resume()
    {
        if (currentTarget == null || indicator == null)
            return;

        if (!currentTarget.gameObject.activeInHierarchy)
            return;

        indicator.gameObject.SetActive(true);
        UpdateIndicatorPosition();
    }

    void UpdateIndicatorPosition()
    {
        if (!currentTarget.gameObject.activeInHierarchy)
        {
            Suspend();
            return;
        }

        RectTransform parent =
            indicator.parent as RectTransform;

        if (parent == null)
            return;

        Vector3 worldPosition =
            currentTarget.TransformPoint(
                new Vector3(
                    0f,
                    currentTarget.rect.yMax,
                    0f));

        Vector3 localPosition =
            parent.InverseTransformPoint(worldPosition);

        animationTime += Time.deltaTime;

        float floating =
            Mathf.Sin(animationTime * floatSpeed)
            * floatHeight;

        indicator.localPosition =
            localPosition +
            new Vector3(
                currentOffset.x,
                currentOffset.y + floating,
                0f);
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}
