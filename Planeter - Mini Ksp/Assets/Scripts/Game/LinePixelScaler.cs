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
        lineRenderer.endWidth = lineRenderer.startWidth = 0f;
    }
    public void Update()
    {

        // Line 
        float width = PixelScaler.Scale(lineWidthPx);
        lineRenderer.endWidth = lineRenderer.startWidth = width;
        // Better Tiling
        if (lineRenderer.textureMode == LineTextureMode.Tile)
        {
            lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
        }
    }

}
