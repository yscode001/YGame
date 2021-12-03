using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#region 单例及初始化
public partial class YCameraControl : MonoBehaviour
{
    private YCameraControl() { }
    public static YCameraControl Instance { get; private set; } = null;
    public void Init() { if (Instance != this) { Instance = this; } }
    private void OnDestroy() { Instance = null; }
}
#endregion
#region 设置玩家及初始化
public partial class YCameraControl
{
    private Vector3 init_offset_pos = new Vector3(0.4292745f, 2.687055f, 4.513196f);
    private Vector3 init_offset_euler = new Vector3(20f, -180f, 0);
    public Transform PlayerT { get; private set; } = null;
    private Transform cameraT;
    private CharacterController cameraCC;
    private void Start()
    {
        if (Instance != this) { Instance = this; }
        StartAction();
    }
    public void SetupPlayer(Transform playerT)
    {
        if (playerT == null || PlayerT == playerT) { return; }
        PlayerT = playerT;
        StartAction();
    }
    private void StartAction()
    {
        cameraT = Camera.main.transform;
        if (cameraT == null || PlayerT == null) { return; }

        Vector3 toEuler = PlayerT.eulerAngles + init_offset_euler;
        Vector3 toPos = PlayerT.position + init_offset_pos;

        targetX = x = toEuler.y;
        targetY = y = ClampAngle(toEuler.x, yMinLimit, yMaxLimit);
        targetDistance = distance = Mathf.Clamp(Vector3.Distance(PlayerT.position, toPos), minDistance, maxDistance);

        cameraT.localEulerAngles = toEuler;
        Vector3 cameraToPos = PlayerT.position + cameraT.rotation * new Vector3(0, 0, -distance);
        cameraT.SetPositionAndRotation(cameraToPos, Quaternion.Euler(toEuler));

        // 第一次，先直接设置pos，再设置CC
        if (cameraCC == null)
        {
            cameraCC = Camera.main.GetOrAddComponent<CharacterController>();
            cameraCC.radius = 0.2f;
            cameraCC.height = 1f;
            cameraCC.center = new Vector3(0, 0.5f, 0);
        }
    }
}
#endregion
#region 属性
public partial class YCameraControl
{
    private int firstPointerId = -1, secondPointerId = -1;
    private Vector2 firstDragPos, secondDragPos;
    private float x = 0, targetX = 0, y = 0, targetY = 0, distance = 0, targetDistance = 0;
    private readonly float minDistance = 1f, maxDistance = 20f;
    private float xVelocity = 1f, yVelocity = 1f, zoomVelocity = 1f;
    private readonly float xSpeed = 0.15f, ySpeed = 0.15f, zoomSpeed = 0.2f;
    private readonly float yMinLimit = 10f, yMaxLimit = 70f;
    private bool isFirstZoom = true;
    private float firstDragOffset = 0;
    private float ClampAngle(float angle, float min, float max)
    {
        angle = angle < -360 ? angle + 360 : angle;
        angle = angle > 360 ? angle - 360 : angle;
        return Mathf.Clamp(angle, min, max);
    }
}
#endregion
#region 操作
public partial class YCameraControl : IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (firstPointerId == -1)
        {
            firstPointerId = eventData.pointerId;
        }
        else if (secondPointerId == -1)
        {
            secondPointerId = eventData.pointerId;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == firstPointerId)
        {
            firstPointerId = -1;
        }
        else if (eventData.pointerId == secondPointerId)
        {
            secondPointerId = -1;
        }
        if (firstPointerId == -1 && secondPointerId == -1)
        {
            isFirstZoom = true;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (firstPointerId != -1 && secondPointerId != -1)
        {
            if (eventData.pointerId == firstPointerId)
            {
                firstDragPos = eventData.position;
            }
            if (eventData.pointerId == secondPointerId)
            {
                secondDragPos = eventData.position;
            }
            float tempDragOffset = Mathf.Abs(firstDragPos.x - secondDragPos.x);
            if (isFirstZoom) // 第一次缩放
            {
                firstDragOffset = tempDragOffset;
                isFirstZoom = false;
            }
            if (tempDragOffset > firstDragOffset) // 两指距离 > 第一次两指距离，放大
            {
                firstDragOffset = tempDragOffset;
                targetDistance -= zoomSpeed;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
            else if (tempDragOffset < firstDragOffset) // 两指距离 < 第一次两指距离，缩小
            {
                firstDragOffset = tempDragOffset;
                targetDistance += zoomSpeed;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
        }
        else if (firstPointerId != -1 || secondPointerId != -1)
        {
            targetX += eventData.delta.x * xSpeed;
            targetY = ClampAngle(targetY - eventData.delta.y * ySpeed, yMinLimit, yMaxLimit);
        }
    }
}
#endregion
#region LateUpdate更新
public partial class YCameraControl
{
    private void LateUpdate()
    {
        if (cameraT == null || PlayerT == null) { return; }
        x = Mathf.SmoothDampAngle(x, targetX, ref xVelocity, 0.03f);
        y = Mathf.SmoothDampAngle(y, targetY, ref yVelocity, 0.03f);
        cameraT.eulerAngles = new Vector3(y, x, 0);

        distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, 0.03f);
        Vector3 cameraToPos = PlayerT.position + cameraT.rotation * new Vector3(0, 0, -distance);
        if (cameraT.localPosition != cameraToPos)
        {
            cameraCC.Move(cameraToPos - cameraT.localPosition);
        }
    }
}
#endregion
#region 切换玩家
public partial class YCameraControl
{
    public void SwitchPlayerTransform(Transform playerT)
    {
        if (playerT == null || PlayerT == playerT) { return; }
        if (PlayerT == null) { SetupPlayer(playerT); return; }
        // 相机会瞬间进入目标位置
        PlayerT = playerT;
        Vector3 cameraToPos = PlayerT.position + cameraT.rotation * new Vector3(0, 0, -distance);
        if (cameraT.localPosition != cameraToPos)
        {
            cameraCC.Move(cameraToPos - cameraT.localPosition);
        }
    }
}
#endregion