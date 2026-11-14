using System.Collections;
using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("Enemy Portrait")]
    [SerializeField] SpriteRenderer portraitRenderer;
    [SerializeField, Min(0.01f)] float hitPortraitSeconds = 0.25f;

    [Header("Screen Shake")]
    [SerializeField] Transform shakeRoot;
    [SerializeField, Min(0.01f)] float shakeSeconds = 0.15f;
    [SerializeField, Min(0f)] float shakeStrength = 0.06f;

    Coroutine feedbackRoutine;

    Sprite originalPortrait;
    Vector3 originalShakePosition;

    bool portraitChanged;
    bool shakeCaptured;

    public void PlayHit(Sprite hitPortrait)
    {
        if (!isActiveAndEnabled)
            return;


        StopFeedback();

        if (portraitRenderer != null && hitPortrait != null)
        {
            originalPortrait = portraitRenderer.sprite;
            portraitRenderer.sprite = hitPortrait;
            portraitChanged = true;
        }

        if (shakeRoot != null)
        {
            originalShakePosition = shakeRoot.localPosition;
            shakeCaptured = true;
        }

        feedbackRoutine = StartCoroutine(FeedbackRoutine());
    }

    IEnumerator FeedbackRoutine()
    {
        float elapsed = 0f;

        float duration = Mathf.Max(
            hitPortraitSeconds,
            shakeSeconds
        );

        while (elapsed < duration)
        {
            if (portraitChanged &&
                elapsed >= hitPortraitSeconds)
            {
                RestorePortrait();
            }

            if (shakeCaptured && shakeRoot != null)
            {
                if (elapsed < shakeSeconds)
                {
                    float strength = shakeStrength *
                        (1f - elapsed / shakeSeconds);

                    Vector2 offset =
                        Random.insideUnitCircle * strength;

                    shakeRoot.localPosition =
                        originalShakePosition +
                        new Vector3(offset.x, offset.y, 0f);
                }
                else
                {
                    RestoreShake();
                }
            }

            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        RestorePortrait();
        RestoreShake();

        feedbackRoutine = null;
    }

    void RestorePortrait()
    {
        if (!portraitChanged)
            return;

        if (portraitRenderer != null)
            portraitRenderer.sprite = originalPortrait;

        portraitChanged = false;
    }

    void RestoreShake()
    {
        if (!shakeCaptured)
            return;

        if (shakeRoot != null)
            shakeRoot.localPosition = originalShakePosition;

        shakeCaptured = false;
    }

    void StopFeedback()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }

        RestorePortrait();
        RestoreShake();
    }

    void OnDisable()
    {
        StopFeedback();
    }
}