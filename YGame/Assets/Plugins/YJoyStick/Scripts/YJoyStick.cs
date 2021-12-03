using System;
using UnityEngine;
using UnityEngine.UI;

public partial class YJoyStick : MonoBehaviour
{
    [Header("点击触发区域")]
    [SerializeField]
    private Image ImgTouch;

    [Header("摇杆方向背景")]
    [SerializeField]
    private Image ImgDirBG;

    [Header("摇杆中心圆点手指")]
    [SerializeField]
    private Image ImgPointer;

    private float pointerR = 0;
    private Vector2 preDir, currentDir, dirBGDefaultPos, beginDragPos = Vector2.zero;

    private bool IsEnableLog = true;

    /// <summary>
    /// 轮盘方向改变事件
    /// </summary>
    public event Action<Vector3> DirChangedAction;

    /// <summary>
    /// 摇杆单例
    /// </summary>
    public static YJoyStick Instance { get; private set; } = null;

    public void Init()
    {
        if (Instance != this) { Instance = this; }
    }
    public void EnableLog(bool IsEnableLog)
    {
        this.IsEnableLog = IsEnableLog;
    }
    private void Start()
    {
        // 单例
        if (Instance != this) { Instance = this; }

        // 控件大小
        float sw = Screen.width;
        float sh = Screen.height;
        ImgTouch.rectTransform.sizeDelta = new Vector2(sw * 0.4f, sh * 0.7f);
        float touchMin = MathF.Min(ImgTouch.rectTransform.sizeDelta.x, ImgTouch.rectTransform.sizeDelta.y);
        ImgDirBG.rectTransform.sizeDelta = new(touchMin * 0.4f, touchMin * 0.4f);
        ImgPointer.rectTransform.sizeDelta = new Vector2(touchMin * 0.2f, touchMin * 0.2f);

        // 数值
        ImgPointer.color = new Color(1, 1, 1, 0.5f);
        pointerR = (ImgDirBG.rectTransform.sizeDelta.x - ImgPointer.rectTransform.sizeDelta.x) * 0.5f;
        dirBGDefaultPos = ImgDirBG.rectTransform.localPosition;

        // 事件
        YJoyStick_ImgTouch imgTouchScript = ImgTouch.gameObject.AddComponent<YJoyStick_ImgTouch>();
        imgTouchScript.OnPointerDownEvent += OnPointerDown;
        imgTouchScript.OnDragEvent += OnDrag;
        imgTouchScript.OnPointerUpEvent += OnPointerUp;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    private void PrintLog(object message)
    {
        if (IsEnableLog)
        {
            Debug.Log(message);
        }
    }
}
public partial class YJoyStick
{
    private void OnPointerDown(Vector2 position)
    {
        PrintLog("摇杆：手指按下");
        beginDragPos = position;
        ImgPointer.color = new Color(1, 1, 1, 1);
        ImgPointer.rectTransform.localPosition = Vector2.zero;

        Vector2 bgPos = position;
        if (bgPos.x < ImgDirBG.rectTransform.sizeDelta.x * 0.5f) { bgPos.x = ImgDirBG.rectTransform.sizeDelta.x * 0.5f; }
        if (bgPos.y < ImgDirBG.rectTransform.sizeDelta.y * 0.5f) { bgPos.y = ImgDirBG.rectTransform.sizeDelta.y * 0.5f; }
        ImgDirBG.rectTransform.position = bgPos;
    }
    private void OnDrag(Vector2 position)
    {
        PrintLog("摇杆：手指拖拽中");
        currentDir = position - beginDragPos;
        if (currentDir.magnitude > pointerR)
        {
            ImgPointer.rectTransform.localPosition = Vector2.ClampMagnitude(currentDir, pointerR);
        }
        else
        {
            ImgPointer.rectTransform.position = position;
        }
    }
    private void OnPointerUp(Vector2 position)
    {
        PrintLog("摇杆：手指抬起");
        currentDir = Vector2.zero;
        ImgPointer.color = new Color(1, 1, 1, 0.5f);
        ImgPointer.rectTransform.localPosition = Vector2.zero;
        ImgDirBG.rectTransform.localPosition = dirBGDefaultPos;
    }
}
public partial class YJoyStick
{
    private void FixedUpdate()
    {
        if (preDir != currentDir)
        {
            preDir = currentDir;
            Vector3 dirNor = (new Vector3(currentDir.x, 0, currentDir.y)).normalized;
            PrintLog($"摇杆：方向改变，当前方向：{dirNor}");
            DirChangedAction?.Invoke(dirNor); ;
        }
    }
}