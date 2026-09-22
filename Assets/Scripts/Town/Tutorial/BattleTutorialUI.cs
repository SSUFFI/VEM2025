using UnityEngine;
using TMPro;
using System.Collections;

public class BattleTutorialUI : MonoBehaviour
{
    public static BattleTutorialUI Inst;

    [Header("Guide")]
    [SerializeField] GameObject guidePanel;
    public GameObject GuidePanel => guidePanel;

    [SerializeField] TMP_Text guideText;
    [SerializeField] CanvasGroup guidePanelCanvasGroup;

    [Header("Text Fade")]
    [SerializeField] float fadeOutDuration = 0.2f;
    [SerializeField] float fadeInDuration = 0.3f;

    [Header("Panel Fade")]
    [SerializeField] float panelFadeDuration = 0.15f;

    Coroutine guideCoroutine;

    void Awake()
    {
        Inst = this;

        if (guideText != null)
            guideText.alpha = 0f;

        if (guidePanelCanvasGroup != null)
            guidePanelCanvasGroup.alpha = 0f;

        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    public void ShowGuide(string message)
    {
        if (guideCoroutine != null)
            StopCoroutine(guideCoroutine);

        guideCoroutine =
            StartCoroutine(ChangeGuideCo(message));
    }

    IEnumerator ChangeGuideCo(string message)
    {
        bool wasHidden =
            guidePanel == null ||
            !guidePanel.activeSelf;

        if (wasHidden)
        {
            if (guidePanel != null)
                guidePanel.SetActive(true);

            if (guidePanelCanvasGroup != null)
            {
                guidePanelCanvasGroup.alpha = 0f;

                float panelTime = 0f;

                while (panelTime < panelFadeDuration)
                {
                    panelTime += Time.deltaTime;

                    guidePanelCanvasGroup.alpha =
                        Mathf.Lerp(
                            0f,
                            1f,
                            panelTime / panelFadeDuration);

                    yield return null;
                }

                guidePanelCanvasGroup.alpha = 1f;
            }
        }

        else if (guidePanelCanvasGroup != null)
        {
            guidePanelCanvasGroup.alpha = 1f;
        }

        if (guideText == null)
        {
            guideCoroutine = null;
            yield break;
        }

        if (guideText.alpha > 0f &&
            !string.IsNullOrEmpty(guideText.text))
        {
            float time = 0f;
            float startAlpha = guideText.alpha;

            while (time < fadeOutDuration)
            {
                time += Time.deltaTime;

                guideText.alpha = Mathf.Lerp(
                    startAlpha,
                    0f,
                    time / fadeOutDuration);

                yield return null;
            }

            guideText.alpha = 0f;
        }

        guideText.text = message;

        float fadeTime = 0f;

        while (fadeTime < fadeInDuration)
        {
            fadeTime += Time.deltaTime;

            guideText.alpha = Mathf.Lerp(
                0f,
                1f,
                fadeTime / fadeInDuration);

            yield return null;
        }

        guideText.alpha = 1f;

        guideCoroutine = null;
    }

    public void HideGuide()
    {
        if (guideCoroutine != null)
            StopCoroutine(guideCoroutine);

        guideCoroutine =
            StartCoroutine(HideGuideCo());
    }

    IEnumerator HideGuideCo()
    {
        if (guideText != null &&
            guideText.alpha > 0f)
        {
            float time = 0f;
            float startAlpha = guideText.alpha;

            while (time < fadeOutDuration)
            {
                time += Time.deltaTime;

                guideText.alpha = Mathf.Lerp(
                    startAlpha,
                    0f,
                    time / fadeOutDuration);

                yield return null;
            }

            guideText.alpha = 0f;
        }

        if (guidePanelCanvasGroup != null &&
            guidePanelCanvasGroup.alpha > 0f)
        {
            float panelTime = 0f;
            float startAlpha =
                guidePanelCanvasGroup.alpha;

            while (panelTime < panelFadeDuration)
            {
                panelTime += Time.deltaTime;

                guidePanelCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        0f,
                        panelTime / panelFadeDuration);

                yield return null;
            }

            guidePanelCanvasGroup.alpha = 0f;
        }

        if (guidePanel != null)
            guidePanel.SetActive(false);

        guideCoroutine = null;
    }

    void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }
}