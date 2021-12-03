using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using YUnity;

public partial class SceneLoader : MonoBehaviour
{
    private SceneLoader() { }
    public static SceneLoader Instance { get; private set; } = null;

    public void Init()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
}
public partial class SceneLoader
{
    public void LoadSceneAsync(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || !SceneNames.All.Contains(sceneName)) { return; }

        // 清理资源
        PoolMag.Instance.Clear();
        UIStackMag.Instance.Clear();

        // 退出旧场景
        Scene exitScene = SceneManager.GetActiveScene();
        OnSceneUnloaded(exitScene);

        // 加载新场景
        SceneMag.Instance.LoadSceneAsync(sceneName, () =>
        {
            WndRoot.Instance.LoadingScene_begin();
        }, (progress) =>
        {
            WndRoot.Instance.LoadingScene_loading(progress);
        }, () =>
        {
            WndRoot.Instance.LoadingScene_complete();
        });
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 为保证先卸载，再加载，我们在LoadSceneAsync中手动调用卸载，而不是使用SceneManager的sceneUnloaded
        // SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // 为保证先卸载，再加载，我们在LoadSceneAsync中手动调用卸载，而不是使用SceneManager的sceneUnloaded
        // SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"场景被退出，SceneName：{scene.name}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景被加载完毕，SceneName：{scene.name}，LoadSceneMode：{mode}");
    }
}