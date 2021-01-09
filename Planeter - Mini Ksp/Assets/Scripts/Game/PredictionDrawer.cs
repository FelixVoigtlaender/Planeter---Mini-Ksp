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
    public Vector2 min;
    public Vector2 max;
    public int systemCount = 0;


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


    public void DrawPath(Predictions predictions)
    {
        int curIndex = predictions.GetCurrentIndex();
        int startIndex = curIndex;
        int count = predictions.PredictionCount()-1;
        int switches = 0;
        // Draw Paths of each System
        for (int steps = 0; steps < count; steps++)
        {
            // All swithces reached
            if (switches >= maxSwitches)
            {
                break;
            }

            int i = predictions.CheckIndex(curIndex + steps);
            int nextI = predictions.CheckIndex(i + 1);
            // System Changed
            if (predictions.GetPredictionI(i).gravitySystem != predictions.GetPredictionI(nextI).gravitySystem)
            {
                subsections[switches].DrawSubsection(predictions, startIndex, i);
                subsections[switches].gameObject.SetActive(true);
                switches++;

                startIndex = nextI;
                continue;
            }
            // Last index reached
            if (steps == count - 1)
            {
                subsections[switches].DrawSubsection(predictions, startIndex, i);
                subsections[switches].gameObject.SetActive(true);
                switches++;

                continue;
            }
        }
        // Disable unused subsections
        for (int i = switches; i < subsections.Length; i++)
        {
            subsections[i].gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        for (int i = 0; i < subsections.Length; i++)
        {
            subsections[i].gameObject.SetActive(false);
        }
    }

    public Vector2 RelativeTimePositionToWorld(OMath.OrbitPrediction curPrediciton, List<OMath.OrbitPrediction> entryPredictions)
    {
        if (curPrediciton.gravitySystem.centerSystem == null)
        {
            return curPrediciton.localPosition;
        }

        //if (entryPrediction == null && curPrediction.gravitySystem.parentSystem == prevPrediction.gravitySystem || entryPredictions.Count == 0)
        foreach (OMath.OrbitPrediction entryPred in entryPredictions)
        {
            if (curPrediciton.gravitySystem == entryPred.gravitySystem)
            {
                Vector2 relativePosition = entryPred.gravitySystem.PointToParentSystem(entryPred.time, curPrediciton.localPosition);
                OMath.OrbitPrediction relativePred = new OMath.OrbitPrediction(curPrediciton.time, relativePosition, curPrediciton.localVelocity);
                relativePred.gravitySystem = entryPred.gravitySystem.centerSystem;
                return RelativeTimePositionToWorld(relativePred, entryPredictions);
            }
        }
        return curPrediciton.gravitySystem.PointToWorld(entryPredictions[0].time, curPrediciton.localPosition);
    }


}
