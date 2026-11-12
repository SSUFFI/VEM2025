
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LobbyManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Scene (Editor Only)")]
    [SerializeField] SceneAsset sceneAsset;
#endif

    [Header("Scene Name (Runtime)")]
    [SerializeField] string sceneName;

    [Header("Profile Notification")]
    [SerializeField] GameObject profileNotification;

    void Awake()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
#endif

        if (profileNotification != null)
            profileNotification.SetActive(false);
    }

    void Update()
    {
        if (profileNotification != null &&
            profileNotification.activeSelf &&
            Input.GetMouseButtonDown(0))
        {
            profileNotification.SetActive(false);
        }
    }

    public void LoadScene()
    {
        if (AccountManager.Inst == null ||
            !AccountManager.Inst.HasCurrentAccount())
        {
            if (profileNotification != null)
                profileNotification.SetActive(true);

            return;
        }

        FadeManager.Instance.ChangeScene(sceneName);
    }
}
