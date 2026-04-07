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

    void Awake()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
#endif
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}