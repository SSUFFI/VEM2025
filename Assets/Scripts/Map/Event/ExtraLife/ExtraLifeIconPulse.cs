using UnityEngine;

public class ExtraLifeIconPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] float minScale = 1f;
    [SerializeField] float maxScale = 1.15f;
    [SerializeField] float speed = 2f;

    Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;

        float scale = Mathf.Lerp(minScale, maxScale, t);

        transform.localScale = originalScale * scale;
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
    }
}