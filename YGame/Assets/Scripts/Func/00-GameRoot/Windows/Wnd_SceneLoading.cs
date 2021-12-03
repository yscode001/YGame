using UnityEngine;
using UnityEngine.UI;

public class Wnd_SceneLoading : MonoBehaviour
{
    [SerializeField]
    [Header("加载进度前景图")]
    private Image fgImg;

    [SerializeField]
    [Header("加载进度提示")]
    private Text tipsText;

    public void Init()
    {
        fgImg.fillAmount = 0;
        tipsText.text = "";
    }
    public void SetupLoadingProgress(float progress)
    {
        progress = Mathf.Clamp(progress, 0, 1);
        fgImg.fillAmount = progress;
        tipsText.text = $"正在玩儿命加载中：{(int)(progress * 100)}% ...";
    }
}