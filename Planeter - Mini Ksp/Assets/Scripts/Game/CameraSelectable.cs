using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelectable : MonoBehaviour
{
    public float pixelRadius = 100;

    public bool selectOnStart = false;

    private void Start()
    {
        if (selectOnStart)
            CameraController.SetTarget(transform);
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ManageInput(Input.GetTouch(0).position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ManageInput(Input.mousePosition);
        }
    }

    public void ManageInput(Vector2 touchPixelPos)
    {
        Vector2 myPixelPos = Camera.main.WorldToScreenPoint(transform.position);

        float pixelDistance = (myPixelPos - touchPixelPos).magnitude;

        if (pixelDistance < pixelRadius)
        {
            CameraController.SetTarget(transform);
        }
    }
}
