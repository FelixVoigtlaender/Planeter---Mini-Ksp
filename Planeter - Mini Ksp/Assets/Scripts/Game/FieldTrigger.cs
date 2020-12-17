using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FieldTrigger : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action<PointerEventData> onDrag;
    public event Action<PointerEventData> onEndDrag;
    public event Action<PointerEventData> onPointerDown;
    public event Action<PointerEventData> onPointerUp;
    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }
}
