using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBody : MonoBehaviour
{
    public float mass = 1;

    public int predictionCount = 1000;

    public OrbitMath.OrbitPrediction startPrediction;



    public bool showLocal = false;
    public int maxIndex;
    public int currentIndex;
    public OrbitMath.OrbitPrediction[] predictions;
    public PredictionDrawer predictionDrawer;

    public void Start()
    {
        predictionDrawer = GetComponent<PredictionDrawer>();

        if(startPrediction != null && startPrediction.gravitySystem)
        {
            startPrediction.localPosition = startPrediction.gravitySystem.PointToSystem(0, transform.position);
        }

        predictions = new OrbitMath.OrbitPrediction[predictionCount];
        for (int i = 0; i < predictions.Length; i++)
        {
            predictions[i] = new OrbitMath.OrbitPrediction();
        }
        predictions[0] = startPrediction;
    }


    public void FixedUpdate()
    {
        //Predict path
        int currentPredictionCount = ModuloDistance(currentIndex, maxIndex, predictions.Length);
        int newPredictionCount = Mathf.Max(100 - currentPredictionCount, 10);
        if (ModuloDistance(currentIndex, maxIndex, predictions.Length) < predictions.Length - newPredictionCount-2)
        {
            predictions = PredictPath(predictions, maxIndex, newPredictionCount);
            maxIndex = (maxIndex + newPredictionCount) % predictions.Length;
        }

        //
        //currentIndex = (currentIndex + 1) % predictions.Length;

        int skipSteps = (int)((OTime.time - predictions[currentIndex].time) / OTime.fixedDeltaTime);
        currentIndex = (currentIndex + skipSteps) % predictions.Length;
        OrbitMath.OrbitPrediction prediction = predictions[currentIndex];

        transform.parent = prediction.gravitySystem.transform;
        transform.localPosition = prediction.localPosition;

        DrawPath(predictions, currentIndex, maxIndex);


        Grapher.Log(prediction.localGravity.magnitude, "Gravity", prediction.time);
        Grapher.Log(prediction.localGravity.magnitude, prediction.gravitySystem.name, prediction.gravitySystem.renderer.color, prediction.time);
    }

    public void DrawPath(OrbitMath.OrbitPrediction[] predictions, int curI, int maxI)
    {
        if (predictionDrawer)
        {
            predictionDrawer.DrawPrediction(predictions, curI, maxI);
            return;

        }

       

        // Predictions in the same system are calculated relative to their entry points!
        List<OrbitMath.OrbitPrediction> entryPredictions = new List<OrbitMath.OrbitPrediction>();
        Vector2 lastPosition = predictions[curI].gravitySystem.PointToWorld(predictions[curI].time, predictions[curI].localPosition);
        for (int steps = 0; steps < ModuloDistance(curI, maxI, predictions.Length) - 1; steps++)
        {
            //Get Index
            int i = (steps + curI) % predictions.Length;
            int prevI = (i - 1 + predictions.Length) % predictions.Length;

            // Current Prediction
            OrbitMath.OrbitPrediction curPrediction = predictions[i];
            // Find Relative entry Predicion
            OrbitMath.OrbitPrediction entryPrediction = null;
            foreach (OrbitMath.OrbitPrediction pred in entryPredictions)
            {
                if(pred.gravitySystem == curPrediction.gravitySystem)
                {
                    entryPrediction = pred;
                }
            }
            // No entry Prediction Found
            if(entryPrediction == null)
            {
                entryPrediction = curPrediction;
                entryPredictions.Add(curPrediction);
            }

            // Get Relative Position
            Vector2 relativePosition = RelativeTimePositionToWorld(curPrediction, entryPredictions);
            if (predictions[prevI].gravitySystem == predictions[i].gravitySystem)
            {
                Debug.DrawLine(lastPosition, relativePosition, curPrediction.gravitySystem.renderer.color);
            }
            lastPosition = relativePosition;
        }
    }

    public Vector2 RelativeTimePositionToWorld(OrbitMath.OrbitPrediction curPrediciton, List<OrbitMath.OrbitPrediction> entryPredictions)
    {
        if(curPrediciton.gravitySystem.parentSystem == null)
        {
            return curPrediciton.localPosition;
        }

        foreach(OrbitMath.OrbitPrediction entryPred in entryPredictions)
        {
            if (curPrediciton.gravitySystem == entryPred.gravitySystem)
            {
                Vector2 relativePosition = entryPred.gravitySystem.PointToParentSystem(entryPred.time, curPrediciton.localPosition);
                OrbitMath.OrbitPrediction relativePred = new OrbitMath.OrbitPrediction(curPrediciton.time, relativePosition, curPrediciton.localVelocity);
                relativePred.gravitySystem = entryPred.gravitySystem.parentSystem;
                return RelativeTimePositionToWorld(relativePred, entryPredictions);
            }
        }
        return curPrediciton.gravitySystem.PointToWorld(entryPredictions[0].time,curPrediciton.localPosition);
    }

    public OrbitMath.OrbitPrediction[] PredictPath(OrbitMath.OrbitPrediction[] predictions, int index, int steps)
    {
        for(int i = 0; i < steps; i++)
        {
            int nextMaxIndex = (index + 1) % predictions.Length;
            predictions[nextMaxIndex] = CalculateNextPrediction(predictions[index]);
            index = nextMaxIndex;
        }
        return predictions;
    }

    int ModuloDistance(int a, int b, int m)
    {
        if (a <= b)
            return b - a;
        else
            return (m - a) + b;
    }

    public OrbitMath.OrbitPrediction CalculateNextPrediction(OrbitMath.OrbitPrediction currentPrediction)
    {
        OrbitMath.OrbitPrediction nextPrediction = currentPrediction.Clone();
        
        // Collision
        if (nextPrediction.localPosition.magnitude < nextPrediction.gravitySystem.radius)
        {
            bool fliesIntoCenter = Vector2.Dot(nextPrediction.localPosition.normalized, nextPrediction.localVelocity) < 0;
            if (fliesIntoCenter)
            {
                Vector2 position = nextPrediction.localPosition.normalized * nextPrediction.gravitySystem.radius;
                nextPrediction.localPosition = position;
                nextPrediction.localVelocity = Vector2.zero;
            }
        }
        // Movement
        float deltaTime = OTime.fixedDeltaTime;
        nextPrediction.time += deltaTime;
        nextPrediction.localVelocity += nextPrediction.localGravity * deltaTime;
        nextPrediction.localPosition += nextPrediction.localVelocity * deltaTime;



        nextPrediction = nextPrediction.gravitySystem.DynamicPrediction(nextPrediction, mass);

        return nextPrediction;
    }

    public void AddVelocity(Vector2 velocity)
    {
        predictions[currentIndex].localVelocity += velocity;
        maxIndex = currentIndex;
    }

    public void PretendAddVelocity(Vector2 velocity)
    {
        OrbitMath.OrbitPrediction[] path = new OrbitMath.OrbitPrediction[500];
        path[0] = predictions[currentIndex].Clone();
        path[0].localVelocity += velocity;

        path = PredictPath(path, 0, 499);
        DrawPath(path, 0, 499);
    }

    private void OnDrawGizmos()
    {

        if (predictions == null)
            return;
        if (predictions.Length < predictionCount)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, predictions[currentIndex].localVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, predictions[currentIndex].localGravity);
    }

    public OrbitMath.OrbitPrediction GetCurrentPrediction()
    {
        return predictions[currentIndex];
    }

    public OrbitMath.OrbitPrediction GetLastPrediction()
    {
        return predictions[maxIndex];
    }
}
