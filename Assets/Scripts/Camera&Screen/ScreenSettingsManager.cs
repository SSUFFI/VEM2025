using UnityEngine;

public class ScreenSettingsManager : MonoBehaviour
{
    public static ScreenSettingsManager Inst { get; private set; }

    private const string WINDOW_MODE_KEY = "VEM_WindowMode";

    private const int WINDOW_WIDTH = 1600;
    private const int WINDOW_HEIGHT = 900;

    public bool IsWindowed { get; private set; }

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID || UNITY_IOS
        return;
#else
        IsWindowed = PlayerPrefs.GetInt(WINDOW_MODE_KEY, 0) == 1;
        ApplyWindowMode();
#endif
    }

    public void SetWindowMode(bool isWindowed)
    {
#if UNITY_ANDROID || UNITY_IOS
        return;
#else
        if (IsWindowed == isWindowed)
            return;

        IsWindowed = isWindowed;

        PlayerPrefs.SetInt(WINDOW_MODE_KEY, IsWindowed ? 1 : 0);
        PlayerPrefs.Save();

        ApplyWindowMode();
#endif
    }

    private void ApplyWindowMode()
    {
#if UNITY_ANDROID || UNITY_IOS
        return;
#else
        if (IsWindowed)
        {
            Screen.SetResolution(
                WINDOW_WIDTH,
                WINDOW_HEIGHT,
                FullScreenMode.Windowed
            );
        }
        else
        {
            Screen.SetResolution(
                Screen.currentResolution.width,
                Screen.currentResolution.height,
                FullScreenMode.FullScreenWindow
            );
        }
#endif
    }
}
