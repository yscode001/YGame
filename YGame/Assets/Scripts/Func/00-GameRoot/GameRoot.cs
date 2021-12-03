using UnityEngine;
using YUnity;

[RequireComponent(typeof(SceneLoader))]
public partial class GameRoot : MonoBehaviour
{
    private GameRoot() { }
    public static GameRoot Instance { get; private set; } = null;
}
public partial class GameRoot
{
    [SerializeField]
    [Header("根Window")]
    private WndRoot wndRoot;

    private SceneLoader sceneLoader;

    private void Start()
    {
        Init();
        HandleLogicAfterInit();
    }
    private void Init()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;

        // YFramework初始化
        GameRootMag.Init(gameObject.scene);

        // 根Wnd初始化
        wndRoot.Init();

        // SceneLoader初始化
        sceneLoader = GetComponent<SceneLoader>();
        sceneLoader.Init();
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    private void HandleLogicAfterInit()
    {
        wndRoot.BeginCheckHotupdate();
    }
}