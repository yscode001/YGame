using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using YUnity;

#region 面板属性
public partial class Wnd_Hotupdate : MonoBehaviour
{
    [Header("检查资源")]
    [SerializeField]
    private RectTransform CheckResourcesRT;

    [SerializeField]
    private Text CheckTipsText;

    [SerializeField]
    [Header("下载资源")]
    private RectTransform DownloadingResourcesRT;

    [SerializeField]
    private Image DownloadingFgImg;

    [SerializeField]
    private Text DownloadingTipsText;

    private long needDownloadSizeByte = 0;
}
#endregion
#region 获取待下载内容大小
public partial class Wnd_Hotupdate
{
    private IEnumerator GetDownloadSizeAction(string label, Action<long> complete)
    {
        var size = Addressables.GetDownloadSizeAsync(label);
        yield return size;
        complete?.Invoke(size.Result);
    }
    private void GetDownloadSize(string label, Action<long> complete)
    {
        if (string.IsNullOrWhiteSpace(label) || !AALabel.all.Contains(label))
        {
            complete?.Invoke(0);
        }
        else
        {
            StartCoroutine(GetDownloadSizeAction(label, complete));
        }
    }
}
#endregion
#region 下载热更内容
public partial class Wnd_Hotupdate
{
    private IEnumerator DownloadDependenciesAction(string label, Action<float> progress)
    {
        var handle = Addressables.DownloadDependenciesAsync(label);
        while (!handle.IsDone)
        {
            var status = handle.GetDownloadStatus();
            progress?.Invoke(status.Percent);
            yield return new WaitForSeconds(0.4f);
        }
        progress?.Invoke(1);
    }
    private void DownloadDependencies(string label, Action<float> progress)
    {
        if (string.IsNullOrWhiteSpace(label) || !AALabel.all.Contains(label))
        {
            progress?.Invoke(1);
        }
        else
        {
            StartCoroutine(DownloadDependenciesAction(label, progress));
        }
    }
}
#endregion
#region 逻辑
public partial class Wnd_Hotupdate
{
    public void Init()
    {
        CheckResourcesRT.SetAct(false);
        DownloadingResourcesRT.SetAct(false);
    }
    public void BeginCheck()
    {
        CheckResourcesRT.SetAct(true);
        DownloadingResourcesRT.SetAct(false);
        CheckTipsText.text = "正在检查资源更新，请稍后...";
        GetDownloadSize(AALabel.remote_all, (size) =>
        {
            needDownloadSizeByte = size;
            if (size == 0)
            {
                CheckResourcesRT.SetAct(false);
                DownloadingResourcesRT.SetAct(false);
                AfterAction();
            }
            else
            {
                BeginDownload();
            }
        });
    }
    private void BeginDownload()
    {
        CheckResourcesRT.SetAct(false);
        DownloadingResourcesRT.SetAct(true);
        DownloadDependencies(AALabel.remote_all, (progress) =>
        {
            DownloadingFgImg.fillAmount = progress;
            int curSize = (int)(needDownloadSizeByte * progress / 1024 / 1024);
            int totalSize = (int)(needDownloadSizeByte / 1024 / 1024);
            DownloadingTipsText.text = $"正在下载资源 ({curSize}M/{totalSize}M)...";
            if (progress == 1)
            {
                CheckResourcesRT.SetAct(false);
                DownloadingResourcesRT.SetAct(false);
                AfterAction();
            }
        });
    }
    private void AfterAction()
    {
        if (CurrentUser.Instance.IsEmpty)
        {
            SceneLoader.Instance.LoadSceneAsync(SceneNames.Login);
        }
        else if (CurrentUser.Instance.gender == (int)TGender.none)
        {
            SceneLoader.Instance.LoadSceneAsync(SceneNames.SelectGender);
        }
        else
        {
            SceneLoader.Instance.LoadSceneAsync(SceneNames.Lobby);
        }
        this.SetAct(false);
    }
}
#endregion