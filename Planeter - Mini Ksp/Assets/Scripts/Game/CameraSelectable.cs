using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSelectable : MonoBehaviour
{
    public float pixelRadius = 100;

    public bool selectOnStart = false;

    public bool useWorldSpace = true;

    public FieldTrigger fieldTrigger;

    public bool useCameraSetup = false;
    public CameraSetup cameraSetup;

    private void Start()
    {
        if (!fieldTrigger)
            fieldTrigger = FindObjectOfType<FieldTrigger>();
        if (fieldTrigger)
        {
            fieldTrigger.onPointerUp += OnPointerUp;
        }
        if (selectOnStart)
        {
            GameManager.OnGameStart += Select;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging && Input.touchCount <2)
            return;

        ManageInput(eventData.position);
    }

    public void ManageInput(Vector2 touchPixelPos)
    {
        if (useWorldSpace)
            ManageWorldInput(Camera.main.ScreenToWorldPoint(touchPixelPos));
        else
            ManagePixelInput(touchPixelPos);
    }

    
    public void ManagePixelInput(Vector2 touchPixelPos)
    {
        Vector2 myPixelPos = Camera.main.WorldToScreenPoint(transform.position);

        float pixelDistance = (myPixelPos - touchPixelPos).magnitude;

        if (pixelDistance < pixelRadius)
        {
            var worldToPixels = ((Screen.height / 2.0f) / Camera.main.orthographicSize);
            Select(pixelDistance/worldToPixels);
        }
    }


    public void Select()
    {
        Select(0,true);
    }
    public void Select(float distance = 0, bool instant = false)
    {
        if (useCameraSetup)
        {
            cameraSetup.target = transform;
            CameraController.instance.SetCameraSetup(cameraSetup);
        }
        else
        {
            CameraController.SetTarget(transform, distance);
        }

        if (instant)
        {
            CameraController.instance.targetDelta = Vector2.zero;
        }
    }

    /// <summary>
    /// Uses lossy scale as radius!
    /// </summary>
    /// <param name="worldPos">Position of click in world space</param>
    public void ManageWorldInput(Vector2 worldPos)
    {
        float worldDistance = ((Vector2)transform.position - worldPos).magnitude;
        float radius = transform.lossyScale.x / 2;

        if (worldDistance < radius)
        {
            Select(worldDistance);
        }
    }


}
