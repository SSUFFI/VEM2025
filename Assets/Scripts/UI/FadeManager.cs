
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("Fade UI")]
    [SerializeField] CanvasGroup fadeGroup;

    [Header("Fade Settings")]
    [SerializeField] float fadeOutDuration = 0.3f;
    [SerializeField] float blackHoldDuration = 0.1f;
    [SerializeField] float fadeInDuration = 0.35f;

    [Header("Scene Settings")]
    [SerializeField] bool fadeInOnSceneStart = true;

    public bool IsTransitioning { get; private set; }

    static bool pendingSceneFadeIn;

    void Awake()
    {
        Instance = this;

        if (fadeGroup == null)
        {
            Debug.LogError("FadeGroup이 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        fadeGroup.alpha = pendingSceneFadeIn ? 1f : 0f;
        fadeGroup.blocksRaycasts = pendingSceneFadeIn;
        fadeGroup.interactable = false;
    }

    IEnumerator Start()
    {
        if (pendingSceneFadeIn)
        {
            pendingSceneFadeIn = false;
            yield return FadeIn();
        }
        else if (fadeInOnSceneStart)
        {
            fadeGroup.alpha = 1f;
            fadeGroup.blocksRaycasts = true;

            yield return FadeIn();
        }
    }

    public void ChangePanel(GameObject currentPanel, GameObject nextPanel)
    {
        if (IsTransitioning || nextPanel == null)
            return;

        StartCoroutine(PanelTransition(currentPanel, nextPanel));
    }

    IEnumerator PanelTransition(GameObject currentPanel, GameObject nextPanel)
    {
        IsTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        yield return fadeGroup
            .DOFade(1f, fadeOutDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        if (currentPanel != null)
            currentPanel.SetActive(false);

        nextPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(blackHoldDuration);

        yield return FadeIn();
    }

    public void ChangeScene(string sceneName)
    {
        if (IsTransitioning || string.IsNullOrEmpty(sceneName))
            return;

        StartCoroutine(SceneTransition(sceneName));
    }

    IEnumerator SceneTransition(string sceneName)
    {
        IsTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        yield return fadeGroup
            .DOFade(1f, fadeOutDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        yield return new WaitForSecondsRealtime(blackHoldDuration);

        pendingSceneFadeIn = true;

        SceneManager.LoadScene(sceneName);
    }

    public void FadeTransition(Action action)
    {
        if (IsTransitioning || action == null)
            return;

        StartCoroutine(FadeActionRoutine(action));
    }

    IEnumerator FadeActionRoutine(Action action)
    {
        IsTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        yield return fadeGroup
            .DOFade(1f, fadeOutDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        action.Invoke();

        yield return new WaitForSecondsRealtime(blackHoldDuration);

        yield return FadeIn();
    }

    IEnumerator FadeIn()
    {
        Debug.Log("Fade In 실행됨!");

        IsTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        yield return fadeGroup
            .DOFade(0f, fadeInDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        fadeGroup.blocksRaycasts = false;
        IsTransitioning = false;
    }

    public void ChangeScene(string sceneName, System.Action beforeLoad)
    {
        if (IsTransitioning || string.IsNullOrEmpty(sceneName))
            return;

        StartCoroutine(SceneTransitionWithAction(sceneName, beforeLoad));
    }

    IEnumerator SceneTransitionWithAction(
        string sceneName,
        System.Action beforeLoad)
    {
        IsTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        yield return fadeGroup
            .DOFade(1f, fadeOutDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        beforeLoad?.Invoke();

        yield return new WaitForSecondsRealtime(blackHoldDuration);

        pendingSceneFadeIn = true;
        SceneManager.LoadScene(sceneName);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (fadeGroup != null)
            fadeGroup.DOKill();
    }
}
