using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SlateBattleIntroUI : MonoBehaviour
{
    [Header("직접 만든 UI 연결 ? 비우면 기본 UI 생성")]
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private Image runeImage;
    [SerializeField] private TMP_Text effectText;

    [Header("이동 위치 ? 선택 사항")]
    [SerializeField] private RectTransform leftAnchor;
    [SerializeField] private RectTransform rightAnchor;

    [Header("연출")]
    [SerializeField, Min(0.1f)]
    private float travelSeconds = 2.4f;

    [SerializeField, Range(0f, 1f)]
    private float centerScaleIncrease = 0.15f;

    [SerializeField, Min(0f)]
    private float shakeDegrees = 3f;

    [SerializeField, Min(0f)]
    private float shakeCycles = 3f;

    [SerializeField, Min(0f)]
    private float textHoldSeconds = 1f;

    [Header("효과음 ? 선택 사항")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;

    private GameObject generatedOverlay;

    public IEnumerator Play(
        SlateDataSO reward,
        Action applyEffect,
        TMP_FontAsset font)
    {
        EnsureUI(font);

        overlay.gameObject.SetActive(true);
        overlay.alpha = 1f;
        overlay.blocksRaycasts = true;

        effectText.text = "";

        runeImage.sprite = reward.RuneIcon;
        runeImage.enabled = reward.RuneIcon != null;
        runeImage.preserveAspect = true;

        RectTransform rect = runeImage.rectTransform;

        Vector3 originalPosition = rect.localPosition;
        Vector3 originalScale = rect.localScale;
        Quaternion originalRotation = rect.localRotation;

        Vector3 from = leftAnchor != null
            ? rect.parent.InverseTransformPoint(leftAnchor.position)
            : new Vector3(-1100f, 100f, 0f);

        Vector3 to = rightAnchor != null
            ? rect.parent.InverseTransformPoint(rightAnchor.position)
            : new Vector3(1100f, 100f, 0f);

        bool applied = false;
        float elapsed = 0f;
        float duration = Mathf.Max(0.1f, travelSeconds);

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // 양 끝: 0 / 중앙: 1
            float center = Mathf.Sin(t * Mathf.PI);

            rect.localPosition = Vector3.Lerp(from, to, t);

            rect.localScale =
                originalScale *
                (1f + centerScaleIncrease * center);

            float shake =
                Mathf.Sin(t * Mathf.PI * 2f * shakeCycles) *
                shakeDegrees *
                center;

            rect.localRotation =
                originalRotation *
                Quaternion.Euler(0f, 0f, shake);

            if (!applied && t >= 0.5f)
            {
                ApplyAtCenter();
                applied = true;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // 프레임 간격이 길어 중앙 지점을 건너뛰어도 한 번 적용합니다.
        if (!applied)
            ApplyAtCenter();

        runeImage.enabled = false;

        yield return new WaitForSecondsRealtime(textHoldSeconds);

        overlay.alpha = 0f;
        overlay.blocksRaycasts = false;
        overlay.gameObject.SetActive(false);

        rect.localPosition = originalPosition;
        rect.localScale = originalScale;
        rect.localRotation = originalRotation;

        void ApplyAtCenter()
        {
            effectText.text =
                reward.rewardName + "\n" +
                reward.EffectDescription;

            if (reward.effectType == SlateEffectType.GrantGold)
            {
                effectText.text +=
                    "\n(석판 선택 시 지급 완료)";
            }

            if (audioSource != null &&
                activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }

            applyEffect?.Invoke();
        }
    }

    private void EnsureUI(TMP_FontAsset font)
    {
        if (overlay != null &&
            runeImage != null &&
            effectText != null)
        {
            return;
        }

        if (overlay != null)
            overlay.gameObject.SetActive(false);

        generatedOverlay = new GameObject(
            "Slate Intro Overlay",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(CanvasGroup));

        Canvas canvas =
            generatedOverlay.GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler =
            generatedOverlay.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight = 0.5f;

        overlay =
            generatedOverlay.GetComponent<CanvasGroup>();

        Image backdrop =
            CreateImage("Backdrop", generatedOverlay.transform);

        backdrop.color =
            new Color(0f, 0f, 0f, 0.45f);

        backdrop.rectTransform.anchorMin = Vector2.zero;
        backdrop.rectTransform.anchorMax = Vector2.one;
        backdrop.rectTransform.offsetMin = Vector2.zero;
        backdrop.rectTransform.offsetMax = Vector2.zero;
        backdrop.raycastTarget = true;

        runeImage =
            CreateImage("Rune", generatedOverlay.transform);

        runeImage.rectTransform.sizeDelta =
            new Vector2(240f, 240f);

        runeImage.raycastTarget = false;

        GameObject textObject = new GameObject(
            "Effect Text",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));

        textObject.transform.SetParent(
            generatedOverlay.transform,
            false);

        effectText =
            textObject.GetComponent<TextMeshProUGUI>();

        if (font != null)
            effectText.font = font;

        effectText.rectTransform.sizeDelta =
            new Vector2(1500f, 200f);

        effectText.rectTransform.anchoredPosition =
            new Vector2(0f, -140f);

        effectText.alignment = TextAlignmentOptions.Center;
        effectText.fontSize = 42f;
        effectText.color = Color.white;
        effectText.raycastTarget = false;
    }

    private static Image CreateImage(
        string objectName,
        Transform parent)
    {
        GameObject child = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Image));

        child.transform.SetParent(parent, false);

        return child.GetComponent<Image>();
    }

    private void OnDestroy()
    {
        if (generatedOverlay != null)
            Destroy(generatedOverlay);
    }
}