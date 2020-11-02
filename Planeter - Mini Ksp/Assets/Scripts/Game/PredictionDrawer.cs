using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

public class PredictionDrawer : MonoBehaviour
{
    public GameObject subsectionPrefab;
    public PathSubsection[] subsections;
    public int maxSwitches = 3;

    private void Start()
    {
        CreateLineObjects();
    }

    void CreateLineObjects()
    {
        subsections = new PathSubsection[maxSwitches];
        for (int i = 0; i < subsections.Length; i++)
        {
            GameObject lineObject = Instantiate(subsectionPrefab);
            subsections[i] = lineObject.GetComponent<PathSubsection>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predictions"></param>
    /// <param name="curI"></param>
    /// <param name="maxI"></param>
    /// <returns>Max Size </returns>
    public Vector2 DrawPrediction(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        Vector2 start = predictions[curI].gravitySystem.PointToWorld(predictions[curI].time, predictions[curI].localPosition);
        Vector2 maxSize = Vector2.zero;

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
                subsections[switches].gameObject.SetActive(true);
                subsections[switches].SetUp(prevPrediction.gravitySystem, currentPath);
                currentPath = new List<Vector3>();
                switches++;
            }


            //Get Relative Position
            Vector2 relativePosition = RelativeTimePositionToWorld(curPrediction, entryPredictions);

            //MaxSize
            Vector2 dist = relativePosition - start;
            maxSize.x = Mathf.Abs(dist.x) > maxSize.x ? Mathf.Abs(dist.x) : maxSize.x;
            maxSize.y = Mathf.Abs(dist.y) > maxSize.y ? Mathf.Abs(dist.y) : maxSize.y;

            // add to path
            currentPath.Add(relativePosition);
        }
        if(currentPath.Count > 0 && switches < maxSwitches)
        {
            subsections[switches].gameObject.SetActive(true);
            subsections[switches].SetUp(predictions[maxI].gravitySystem, currentPath);
            switches++;
        }
        for (int i = switches; i < subsections.Length; i++)
        {
            subsections[i].gameObject.SetActive(false);
        }

        return maxSize;
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
