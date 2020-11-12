using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSubsection : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer lineRenderer;
    [Range(0,1f)]
    public float alpha = 1;
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
        {
            gameObject.SetActive(false);
            return;
        }

        Color color = gravitySystem.renderer.color;
        color.a = alpha;

        // Line
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
        lineRenderer.startColor = lineRenderer.endColor = color;



        // EntryPoint
        entryPoint.transform.position = path[0];
        entryPoint.color = color;
        // ExitPoint
        exitPoint.transform.position = path[path.Count - 1];
        exitPoint.color = color;
    }
}
