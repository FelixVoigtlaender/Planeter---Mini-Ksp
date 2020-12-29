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

    public void DrawSubsection(Predictions predictions, int startI, int endI){
        //Convert Predictions to Vector3 Array
        int count =predictions.ModuloDistance(startI,endI) - 1;
        if(count <=0)
        {
            gameObject.SetActive(false);
            return;
        }
        Vector3[] path = new Vector3[count];
        for(int steps = 0; steps < count; steps++)
        {
            int i = predictions.CheckIndex(startI + steps);
            path[steps] = predictions.GetPredictionI(i).localPosition;
        }

        // Set GravitySystem as parent
        transform.parent = predictions.GetPredictionI(startI).gravitySystem.transform;
        transform.localPosition = Vector2.zero;

        //Color
        Color color = predictions.GetPredictionI(startI).gravitySystem.renderer.color;
        color.a = alpha;

        // Set Linerenderer
        lineRenderer.positionCount = count;
        lineRenderer.SetPositions(path);
        lineRenderer.startColor = lineRenderer.endColor = color;
        
        // Set Entry Point
        entryPoint.transform.localPosition = path[0];
        entryPoint.color = color;

        // Set Exit Point
        exitPoint.transform.localPosition = path[path.Length - 1];
        exitPoint.color = color;
    }
}
