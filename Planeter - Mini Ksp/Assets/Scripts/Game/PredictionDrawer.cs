using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PredictionDrawer : MonoBehaviour
{
    public GameObject linePrefab;
    public LineRenderer[] lineObjects;
    public int maxSwitches = 3;

    private void Start()
    {
        CreateLineObjects();
    }

    void CreateLineObjects()
    {
        lineObjects = new LineRenderer[maxSwitches];
        for (int i = 0; i < lineObjects.Length; i++)
        {
            GameObject lineObject = Instantiate(linePrefab);
            lineObjects[i] = lineObject.GetComponent<LineRenderer>();
        }
    }

    public void DrawPrediction(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        // Predictions in the same system are calculated relative to their entry points!
        List<OrbitMath.OrbitPrediction> entryPredictions = new List<OrbitMath.OrbitPrediction>();
        Vector2 lastPosition = predictions[curI].gravitySystem.PointToWorld(predictions[curI].time, predictions[curI].localPosition);
        List<Vector3> currentPath = new List<Vector3>();
        int switches = 0;
        for (int steps = 0; steps < OrbitMath.ModuloDistance(curI, maxI, predictions.Length) - 1; steps++)
        {
            //Get Index
            int i = (steps + curI) % predictions.Length;
            int prevI = (i - 1 + predictions.Length) % predictions.Length;

            // Current Prediction
            OrbitMath.OrbitPrediction prevPrediction = predictions[prevI];
            OrbitMath.OrbitPrediction curPrediction = predictions[i];

            // Find entry prediction
            OrbitMath.OrbitPrediction entryPrediction = null;
            foreach (OrbitMath.OrbitPrediction pred in entryPredictions)
            {
                if (pred.gravitySystem == curPrediction.gravitySystem)
                {
                    entryPrediction = pred;
                }
            }
            // No entry Prediction Found -> Thus switched System
            if (entryPrediction == null && curPrediction.gravitySystem.parentSystem == prevPrediction.gravitySystem || entryPredictions.Count == 0)
            //if (entryPrediction == null)
            {
                entryPrediction = curPrediction;
                entryPredictions.Add(entryPrediction);
            }


            if(curPrediction.gravitySystem != prevPrediction.gravitySystem && switches<maxSwitches)
            {
                lineObjects[switches].gameObject.SetActive(true);
                lineObjects[switches].positionCount = currentPath.Count;
                lineObjects[switches].SetPositions(currentPath.ToArray());
                lineObjects[switches].endColor = prevPrediction.gravitySystem.renderer.color;
                lineObjects[switches].startColor = lineObjects[switches].endColor;
                currentPath = new List<Vector3>();
                switches++;
            }


            //Get Relative Position
            Vector2 relativePosition = RelativeTimePositionToWorld(curPrediction, entryPredictions);
            // add to path
            currentPath.Add(relativePosition);
        }
        if(currentPath.Count > 0 && switches < maxSwitches)
        {

            lineObjects[switches].gameObject.SetActive(true);
            lineObjects[switches].positionCount = currentPath.Count;
            lineObjects[switches].SetPositions(currentPath.ToArray());
            lineObjects[switches].endColor = predictions[maxI].gravitySystem.renderer.color;
            lineObjects[switches].startColor = lineObjects[switches].endColor;
            switches++;
        }
        for (int i = switches; i < lineObjects.Length; i++)
        {
            lineObjects[i].gameObject.SetActive(false);
        }
    }

    public Vector2 RelativeTimePositionToWorld(OrbitMath.OrbitPrediction curPrediciton, List<OrbitMath.OrbitPrediction> entryPredictions)
    {
        if (curPrediciton.gravitySystem.parentSystem == null)
        {
            return curPrediciton.localPosition;
        }

        //if (entryPrediction == null && curPrediction.gravitySystem.parentSystem == prevPrediction.gravitySystem || entryPredictions.Count == 0)
        foreach (OrbitMath.OrbitPrediction entryPred in entryPredictions)
        {
            if (curPrediciton.gravitySystem == entryPred.gravitySystem)
            {
                Vector2 relativePosition = entryPred.gravitySystem.PointToParentSystem(entryPred.time, curPrediciton.localPosition);
                OrbitMath.OrbitPrediction relativePred = new OrbitMath.OrbitPrediction(curPrediciton.time, relativePosition, curPrediciton.localVelocity);
                relativePred.gravitySystem = entryPred.gravitySystem.parentSystem;
                return RelativeTimePositionToWorld(relativePred, entryPredictions);
            }
        }
        return curPrediciton.gravitySystem.PointToWorld(entryPredictions[0].time, curPrediciton.localPosition);
    }


}
