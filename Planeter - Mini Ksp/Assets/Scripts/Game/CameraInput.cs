using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraInput : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public CameraController cameraController;
    private void Start()
    {
        cameraController = CameraController.instance;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //cameraController.ManageDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //cameraController.ManageDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("POINTER DOWN");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("POINTER UP");
    }
}
