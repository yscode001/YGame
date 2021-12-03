using UnityEngine;
using YUnity;

public partial class WndRoot : MonoBehaviour
{
    private WndRoot() { }
    public static WndRoot Instance { get; private set; } = null;
}
public partial class WndRoot
{
    [SerializeField]
    [Header("热更")]
    private Wnd_Hotupdate hotupdate;

    [SerializeField]
    [Header("Scene加载进度条")]
    private Wnd_SceneLoading sceneLoading;

    [SerializeField]
    [Header("Tips弹框")]
    private Wnd_Tips tips;

    public void Init()
    {
        Instance = this;

        hotupdate.Init();
        sceneLoading.Init();
        tips.Init();

        hotupdate.SetAct(false);
        sceneLoading.SetAct(false);
        tips.SetAct(false);
    }
    private void OnDestroy()
    {
        Instance = null;
    }
}
#region 热更
public partial class WndRoot
{
    public void BeginCheckHotupdate()
    {
        hotupdate.SetAct(true);
        hotupdate.BeginCheck();
    }
}
#endregion
#region 场景加载进度条
public partial class WndRoot
{
    public void LoadingScene_begin()
    {
        sceneLoading.SetAct(true);
    }
    public void LoadingScene_loading(float progress)
    {
        sceneLoading.SetupLoadingProgress(progress);
    }
    public void LoadingScene_complete()
    {
        sceneLoading.SetAct(false);
    }
}
#endregion
#region 提示信息
public partial class WndRoot
{
    public void ShowTips(string tips)
    {
        this.tips.ShowTips(tips);
    }
}
#endregion