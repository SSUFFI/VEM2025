using UnityEngine;
using UnityEngine.UI;

public class WindowModeSettingsUI : MonoBehaviour
{
    [Header("Window Mode Toggle")]
    [SerializeField] private Toggle windowModeToggle;

    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (windowModeToggle != null)
            windowModeToggle.gameObject.SetActive(false);
#endif
    }

    private void OnEnable()
    {
#if UNITY_ANDROID || UNITY_IOS
        return;
#else
        if (windowModeToggle == null ||
            ScreenSettingsManager.Inst == null)
            return;

        windowModeToggle.SetIsOnWithoutNotify(
            ScreenSettingsManager.Inst.IsWindowed
        );

        windowModeToggle.onValueChanged.AddListener(OnWindowModeChanged);
#endif
    }

    private void OnDisable()
    {
        if (windowModeToggle != null)
        {
            windowModeToggle.onValueChanged.RemoveListener(
                OnWindowModeChanged
            );
        }
    }

    private void OnWindowModeChanged(bool isWindowed)
    {
        if (ScreenSettingsManager.Inst == null)
            return;

        ScreenSettingsManager.Inst.SetWindowMode(isWindowed);
    }
}
