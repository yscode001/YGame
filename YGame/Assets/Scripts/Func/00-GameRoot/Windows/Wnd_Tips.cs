using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YUnity;

#region 属性与初始化
public partial class Wnd_Tips : MonoBehaviour
{
    [SerializeField]
    [Header("Tips动画")]
    private Animation tipsAni;

    [SerializeField]
    [Header("Tips文本")]
    private Text tipsText;

    private readonly Queue<string> tipsQueue = new Queue<string>();
    private Coroutine playEndCoroutine;

    public void Init()
    {
        ClearAllTips();
    }
}
#endregion
#region 显示与清除Tips
public partial class Wnd_Tips
{
    public void ShowTips(string tips)
    {
        this.SetAct(true);
        lock (tipsQueue)
        {
            tipsQueue.Enqueue(tips);
        }
    }
    public void ClearAllTips()
    {
        lock (tipsQueue)
        {
            while (tipsQueue.Count > 0)
            {
                tipsQueue.Dequeue();
            }
        }
        if (playEndCoroutine != null)
        {
            StopCoroutine(playEndCoroutine);
            playEndCoroutine = null;
        }
        if (tipsAni.isPlaying)
        {
            tipsAni.Stop();
        }
        tipsText.SetAct(false);
        this.SetAct(false);
    }
}
#endregion
#region 动画驱动
public partial class Wnd_Tips
{
    private void Update()
    {
        if (tipsQueue.Count > 0)
        {
            lock (tipsQueue)
            {
                SetTips(tipsQueue.Dequeue());
            }
        }
    }
    private void EndPrevious()
    {
        if (playEndCoroutine != null)
        {
            StopCoroutine(playEndCoroutine);
            playEndCoroutine = null;
        }
        if (tipsAni.isPlaying)
        {
            tipsAni.Stop();
        }
    }
    private void SetTips(string tips)
    {
        EndPrevious();
        tipsText.SetAct(true);
        tipsText.text = tips;

        AnimationClip clip = tipsAni.GetClip("TipsShowAni");
        tipsAni.Play();

        if (playEndCoroutine != null)
        {
            StopCoroutine(playEndCoroutine);
        }
        playEndCoroutine = StartCoroutine(AniPlayDone(clip.length, () =>
        {
            tipsText.SetAct(false);
            this.SetAct(tipsQueue.Count > 0);
        }));
    }
    private IEnumerator AniPlayDone(float second, Action complete)
    {
        yield return new WaitForSeconds(second);
        complete?.Invoke();
    }
}
#endregion