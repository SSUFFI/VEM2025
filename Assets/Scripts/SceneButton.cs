using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public enum TargetMode { ByName, ByBuildIndex, CurrentNext, CurrentReload }

    [Header("Target")]
    public TargetMode target = TargetMode.ByName;
    public string sceneName = "Battle";  // ByName일 때 사용
    public int buildIndex = 0;           // ByBuildIndex일 때 사용

    [Header("Load Options")]
    public LoadSceneMode loadMode = LoadSceneMode.Single;
    public bool useAsync = false;        // 비동기 로딩
    public float delaySeconds = 0f;      // 클릭 후 지연(페이드용)

    // 버튼 OnClick에 이 메서드를 연결하세요.
    public void Load() => StartCoroutine(CoLoad());

    public void LoadByName(string name) { target = TargetMode.ByName; sceneName = name; Load(); }
    public void LoadByIndex(int index) { target = TargetMode.ByBuildIndex; buildIndex = index; Load(); }
    public void LoadNext() { target = TargetMode.CurrentNext; Load(); }
    public void Reload() { target = TargetMode.CurrentReload; Load(); }

    System.Collections.IEnumerator CoLoad()
    {
        if (delaySeconds > 0f) yield return new WaitForSecondsRealtime(delaySeconds);

        switch (target)
        {
            case TargetMode.ByBuildIndex:
                if (useAsync) SceneManager.LoadSceneAsync(buildIndex, loadMode);
                else SceneManager.LoadScene(buildIndex, loadMode);
                break;

            case TargetMode.CurrentNext:
                int next = SceneManager.GetActiveScene().buildIndex + 1;
                if (useAsync) SceneManager.LoadSceneAsync(next, loadMode);
                else SceneManager.LoadScene(next, loadMode);
                break;

            case TargetMode.CurrentReload:
                int cur = SceneManager.GetActiveScene().buildIndex;
                if (useAsync) SceneManager.LoadSceneAsync(cur, loadMode);
                else SceneManager.LoadScene(cur, loadMode);
                break;

            default: // ByName
                if (useAsync) SceneManager.LoadSceneAsync(sceneName, loadMode);
                else SceneManager.LoadScene(sceneName, loadMode);
                break;
        }
    }
}
