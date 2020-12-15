using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelectable : MonoBehaviour
{
    public float pixelRadius = 100;

    public bool selectOnStart = false;

    public bool useWorldSpace = true;


    

    private void Start()
    {
        if (selectOnStart)
            CameraController.SetTarget(transform);
    }

    public void OnInput()
    {
        // Phone Input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ManageInput(Input.GetTouch(0).position);
        }
        // Mouse Input
        if (Input.GetMouseButtonDown(0))
        {
            ManageInput(Input.mousePosition);
        }
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
            CameraController.SetTarget(transform);
        }
    }
    /// <summary>
    /// Uses lossy scale as radius!
    /// </summary>
    /// <param name="worldPos">Position of click in world space</param>
    public void ManageWorldInput(Vector2 worldPos)
    {
        float pixelDistance = ((Vector2)transform.position - worldPos).magnitude;
        float radius = transform.lossyScale.x / 2;

        if (pixelDistance < radius)
        {
            CameraController.SetTarget(transform);
        }
    }


}
