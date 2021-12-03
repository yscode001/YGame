using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class YJoyStick_ImgTouch : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public event Action<Vector2> OnPointerDownEvent;
    public event Action<Vector2> OnDragEvent;
    public event Action<Vector2> OnPointerUpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke(eventData.position);
    }
}