using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSubsection : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer lineRenderer;
    [Header("Points")]
    public SpriteRenderer entryPoint;
    public SpriteRenderer exitPoint;
    public float pointSizePx = 20;

    public Vector2 min;
    public Vector2 max;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }



    public void SetUp(GravitySystem gravitySystem, List<Vector3> path)
    {
        if (path.Count == 0)
            return;

        // Line
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
        lineRenderer.startColor = lineRenderer.endColor = gravitySystem.renderer.color;



        // EntryPoint
        entryPoint.transform.position = path[0];
        entryPoint.color = gravitySystem.renderer.color;
        // ExitPoint
        exitPoint.transform.position = path[path.Count - 1];
        exitPoint.color = gravitySystem.renderer.color;
    }
}
