using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePixelScaler : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer lineRenderer;
    public float lineWidthPx = 3;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void Update()
    {
        // Line 
        float width = PixelScaler.Scale(lineWidthPx);
        lineRenderer.endWidth = lineRenderer.startWidth = width;
    }

}
